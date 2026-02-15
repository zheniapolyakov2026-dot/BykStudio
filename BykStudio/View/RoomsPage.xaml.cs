using static  Android.Provider.DocumentsContract;

namespace BykStudio.View;

public partial class RoomsPage : ContentPage
{
    public List<Room> Rooms { get; set; }

    public RoomsPage()
    {
        InitializeComponent();

        Rooms = new List<Room>
        {
            new Room { Name = "Pink Dream", PricePerHour = 2500, Description = "Нежная комната с неоновыми вывесками.", MainImageUrl = "room1.jpg", Photos = new List<string> { "room1_1.jpg", "room1_2.jpg", "room1_3.jpg" } },
            new Room { Name = "Gold Lux", PricePerHour = 3500, Description = "Классический стиль с розовым золотом.", MainImageUrl = "room2.jpg", Photos = new List<string> { "room2_1.jpg", "room2_2.jpg" } },
            new Room { Name = "Soft Minimal", PricePerHour = 2000, Description = "Минимализм в пастельных тонах.", MainImageUrl = "room3.jpg", Photos = new List<string> { "room3_1.jpg", "room3_2.jpg" } }
        };

        RoomsCollectionView.ItemsSource = Rooms;
    }

    // Открытие галереи
    private void OnViewPhotosClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var room = (Room)button.BindingContext; // Получаем данные комнаты, к которой относится кнопка

        if (room?.Photos != null)
        {
            ImageCarousel.ItemsSource = room.Photos;
            ImageCarousel.IndicatorView = ImageIndicator;
            GalleryOverlay.IsVisible = true;
        }
    }

    private void OnCloseGalleryClicked(object sender, EventArgs e)
    {
        GalleryOverlay.IsVisible = false;
    }

    private async void OnBookClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var room = (Room)button.BindingContext;
        await DisplayAlert("Бронирование", $"Вы выбрали зал: {room.Name}. Переход к оплате...", "OK");
    }
}