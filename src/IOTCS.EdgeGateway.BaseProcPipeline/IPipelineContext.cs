namespace IOTCS.EdgeGateway.BaseProcPipeline
{
    public interface IPipelineContext
    {
        void SendPayload(RouterMessage router);
    }
}
