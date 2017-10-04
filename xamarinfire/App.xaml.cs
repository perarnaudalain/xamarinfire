using Xamarin.Forms;

namespace xamarinfire
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new FirePage();
        }
    }
}
