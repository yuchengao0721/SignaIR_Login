using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UtilHelp;

namespace SignaIR
{
    /// <summary>
    /// SignaIR测试类
    /// </summary>
    public class ChatHub: Hub
    {
        /// <summary>
        /// SendMsg用于前端调用
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public void SendMsg()
        {
            //在客户端实现此处的Show方法
            while (true)
            {
                Thread.Sleep(5000);
                Clients.All.SendAsync("还活着呢");
            }
        }
    }
}
