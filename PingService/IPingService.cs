namespace PingService
{
    [ServiceContract]
    public interface IPingService
    {
        [OperationContract]
        Task<string> GetData(int value);

        
    }

}