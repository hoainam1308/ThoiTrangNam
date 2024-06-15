using ThoiTrangNam.Models;

namespace ThoiTrangNam.Repository
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(VnPaymentResquestModel model, HttpContext context);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
