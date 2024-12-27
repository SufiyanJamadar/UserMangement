using LearnApi.Modal;

namespace LearnApi.Service
{
    public interface IEmailService
    {
       Task SendEmail(Mailrequest mailrequest);
    }
}
