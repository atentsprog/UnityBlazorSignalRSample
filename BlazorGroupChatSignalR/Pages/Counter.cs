
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
namespace BlazorChat.Pages
{
    public partial class Counter : ComponentBase
    {
        private int currentCount = 0;

        private void IncrementCount()
        {
            currentCount++;
        }
    }
}