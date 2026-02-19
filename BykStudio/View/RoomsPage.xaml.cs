using BykStudio.data.Models;

namespace BykStudio.View;

public partial class RoomsPage : ContentPage
{
    public List<Room> Rooms { get; set; }

    public RoomsPage()
    {
        InitializeComponent();

        Rooms = new List<Room>
        {
            new Room {
                Name = "комната 1",
                PricePerHour = 2000,
                Description = "описание.",
                MainImageUrl = "room1.jpg", 
                // Теперь 4 фотографии
                Photos = new List<string> { "room1.jpg", "room2.jpg", "room3.jpg", "room4.jpg" }
            },
            new Room {
                Name = "комната 2",
                PricePerHour = 2000,
                Description = "описание.",
                MainImageUrl = "room2.jpg", 
                // Теперь 3 фотографии
                Photos = new List<string> { "room7.jpg", "room5.jpg", "room6.jpg" }
            }
        };

        RoomsCollectionView.ItemsSource = Rooms;
    }

    private void OnViewPhotosClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var room = (Room)button.BindingContext;
        if (room?.Photos != null)
        {
            ImageCarousel.ItemsSource = room.Photos;
            GalleryOverlay.IsVisible = true;
        }
    }

    private void OnCloseGalleryClicked(object sender, EventArgs e) => GalleryOverlay.IsVisible = false;

    [Obsolete]
    private async void OnBookClicked(object sender, EventArgs e)
    {
        var room = (Room)((Button)sender).BindingContext;
        await DisplayAlert("Бронирование", $"Вы выбрали {room.Name}", "OK");
    }
}