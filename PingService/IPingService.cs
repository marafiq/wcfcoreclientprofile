namespace PingService
{
    [ServiceContract]
    public interface IPingService
    {
        [OperationContract]
        string GetData(int value);

        
    }

}