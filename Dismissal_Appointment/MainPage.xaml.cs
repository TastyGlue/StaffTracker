namespace Dismissal_Appointment
{
    public partial class MainPage : ContentPage
    {
        public MainPage(DatabaseInitializer databaseInitializer)
        {
            InitializeComponent();

            // Initialize the database
            Task.Run(async () =>
            {
                await databaseInitializer.InitializeAsync();
                await databaseInitializer.SeedTestDataAsync();
            });
        }
    }
}
