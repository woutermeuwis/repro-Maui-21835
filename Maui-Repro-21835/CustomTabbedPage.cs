namespace Maui_Repro_21835;

public class CustomTabbedPage : TabbedPage
{
	private ContentView _tabbar;
	public ContentView TabBar => _tabbar ??= BuildTabbar();

	private ContentView BuildTabbar()
	{
		var bar = new ContentView
		{
			HeightRequest = 70d,
			VerticalOptions = LayoutOptions.Center,
			BackgroundColor = Colors.Transparent
		};

		var grid = new Grid
		{
			RowSpacing = 0,
			ColumnSpacing = 0,
			BackgroundColor = Colors.Transparent,
			HorizontalOptions = LayoutOptions.Fill,
			RowDefinitions = [new(new(1, GridUnitType.Star))],
			ColumnDefinitions =
			[
				new(new(1, GridUnitType.Star)),
				new(new(1, GridUnitType.Star)),
				new(new(1, GridUnitType.Star)),
				new(new(1, GridUnitType.Star)),
				new(new(1, GridUnitType.Star))
			]
		};

		var frame = new Frame
		{
			Margin = new(10),
			Padding = Thickness.Zero,
			CornerRadius = 25,
			HorizontalOptions = LayoutOptions.Fill,
			VerticalOptions = LayoutOptions.Fill
		};
		grid.AddWithSpan(frame, 0, 0, 1, 5);

		for (var i = 0; i < 5; i++)
		{
			var tab = new ContentView
			{
				Content = new Label
				{
					Text = (i + 1).ToString(),
					HorizontalOptions = LayoutOptions.Center,
					VerticalOptions = LayoutOptions.Center
				}
			};
			grid.Add(tab, i, 0);
		}

		bar.Content = grid;
		return bar;
	}
}