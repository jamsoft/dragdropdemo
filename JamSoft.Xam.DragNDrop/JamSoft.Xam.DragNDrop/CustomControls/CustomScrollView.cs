using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;

namespace JamSoft.Xam.DragNDrop.CustomControls
{
    public class CustomScrollView : ScrollView
    {
        public CustomScrollView()
        {
            Timer = new System.Timers.Timer();
            Timer.Elapsed += new ElapsedEventHandler(Scroll);
            Timer.Interval = 50;
        }

        public Timer Timer { get; set; }

        public void StartScroll(int scrollDistance)
        {
            if (Timer.Enabled)
            {
                return;
            }

            ScrollDistance = scrollDistance;
            Timer.Enabled = true;
        }

        public int ScrollDistance { get; set; }

        public void StopScroll()
        {
            Timer.Enabled = false;
        }

        private void Scroll(object source, ElapsedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await ScrollToAsync(ScrollX + ScrollDistance, 0, true);
            });
        }
    }
}
