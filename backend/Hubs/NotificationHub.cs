using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace TutoriasDeClases.Hubs
{
    public class NotificationHub : Hub
    {
        // Opcional: métodos invocables por el cliente pueden añadirse aquí

        public override Task OnConnectedAsync()
        {
            // El usuario estará identificado por el claim NameIdentifier (Id de usuario)
            return base.OnConnectedAsync();
        }
    }
}
