using UserService.Dtos;

namespace user_service_refwebsoftware.AsyncDataClient
{
    public interface IMessageBusClient
    {
         void UpdatedUser(UserUpdateAsyncDto userUpdatedDto);
    }
}