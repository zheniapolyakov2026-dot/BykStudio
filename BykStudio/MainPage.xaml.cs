namespace BykStudio;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    private async void OnRoomsButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///rooms");
    }
}