using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Parakeet.Net.Web.Pages;

public class IndexModel : NetPageModel
{
    public void OnGet()
    {

    }

    public async Task OnPostLoginAsync()
    {
        await HttpContext.ChallengeAsync("oidc");
    }
}
