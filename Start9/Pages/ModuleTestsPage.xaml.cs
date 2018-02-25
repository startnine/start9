using System;
using System.AddIn.Hosting;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Start9.Host.AddInView;

namespace Start9.Pages
{
	/// <summary>
	///     Interaction logic for ModuleTestsPage.xaml
	/// </summary>
	public partial class ModuleTestsPage : Page
	{
		private readonly Collection<AddInToken> _addins;

		public ModuleTestsPage()
		{
			InitializeComponent();

			var (addins, warnings) = AddInManager.LoadAddins();

			_addins = addins;

			foreach (string warning in warnings)
			{
				MessageBox.Show(warning, "Add-in Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}

			foreach (AddInToken addin in _addins)
			{
				Addins.Items.Add(new ListViewItem {Content = addin.Name});
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var manager = _addins[Addins.SelectedIndex].Activate<LibraryManager>(new AddInProcess(), AddInSecurityLevel.FullTrust);

			IList<BookInfo> books = AddInManager.CreateBooks();
			books.Add(manager.BestSeller);
			manager.ProcessBooks(books);
			foreach (BookInfo book in books)
			{
				var item = new ListViewItem {Content = $"{book.Title}: {book.Author}"};
				item.MouseDoubleClick += (o, args) =>
					MessageBox.Show(
						$@"{book.Description}
Genre: {book.Genre}
Published on: {book.PublishDate}
Price: ${book.Price}",
						$"Info about {book.Title} by {book.Author}", MessageBoxButton.OK, MessageBoxImage.Information);

				if (string.Equals(book.Id, manager.BestSeller.Id, StringComparison.Ordinal))
				{
					item.Resources.MergedDictionaries.Add(new ResourceDictionary
					{
						Source = new Uri("/Start9.Api;component/Themes/Colors/PlexGreen.xaml", UriKind.Relative)
					});

				}

				Books.Items.Add(item);
			}
		}
	}
}