using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using UserService.Dtos;

namespace user_service_refwebsoftware.AsyncDataClient
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;

        private readonly IConnection _connection;

        private readonly IModel _channel;
        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"])
            };
            try
            {
                _connection = factory.CreateConnection();

                //Le channel correspond à une queue
                _channel = _connection.CreateModel();

                //On declare le type de traitement du channel
                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

                //Gestion de la perte de connexion
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                Console.WriteLine("--> Connected to MessageBus");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}");
            }
        }

        //Cette  méthode transforme les données en texte pour être envoyé à RabbitMQ (RabbitMQ n'accepte que du texte)
        public void UpdatedUser(UserUpdateAsyncDto userUpdatedDto)
        {
            var message = JsonSerializer.Serialize(userUpdatedDto);
            if(_connection.IsOpen)
            {
                Console.WriteLine("--> Updating Client to MessageBus");
                // Envoi du message au bus
                SendMessage(message);
            }
            else
            {
                Console.WriteLine("--> RabbitMq connection is closed");
            }
        }

        // SendMessage envoie les données codées en UTF8 dans le channel
        private  void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "trigger",
                                  routingKey: "",
                                  basicProperties: null,
                                  body: body);
            Console.WriteLine("--> Message sent to MessageBus: ", message);
        }

        public void Dispose()
        {
            if(_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> Connection to MessageBus lost");
        }
    }
}