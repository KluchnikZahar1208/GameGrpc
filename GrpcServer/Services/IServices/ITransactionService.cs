namespace GrpcServer.Services.IServices
{
    public interface ITransactionService
    {
        void MakeTransaction(int matchId);
    }
}
