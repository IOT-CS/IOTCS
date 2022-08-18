namespace IOTCS.EdgeGateway.BaseDriver
{
    public interface IDriver
    {
        bool Connect(string deviceID);

        string Run(string deviceID, string groupID);

        bool IsAviable();
    }
}
