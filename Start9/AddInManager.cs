using System;
using System.AddIn.Hosting;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using Start9.Host.AddInView;

namespace Start9
{
	internal static class AddInManager
	{
		public static (Collection<AddInToken> addins, string[] warnings) LoadAddins()
		{
			string pipeRoot = Path.Combine(Environment.ExpandEnvironmentVariables("%appdata%"), "Start9");

			return (AddInStore.FindAddIns(typeof(LibraryManager), pipeRoot), AddInStore.Update(pipeRoot));
		}

		internal static IList<BookInfo> CreateBooks()
		{
			var books = new List<BookInfo>();

			var paramAuthor = "";
			var paramTitle = "";
			var paramGenre = "";
			var paramPrice = "";
			var paramPublishDate = "";
			var paramDescription = "";

			var xDoc = new XmlDocument();
			xDoc.Load(@".\Books.xml");

			XmlNode xRoot = xDoc.DocumentElement;
			if (xRoot.Name == "catalog")
			{
				XmlNodeList bklist = xRoot.ChildNodes;
				foreach (XmlNode bk in bklist)
				{
					string paramId = bk.Attributes[0].Value;
					XmlNodeList dataItems = bk.ChildNodes;
					int items = dataItems.Count;
					foreach (XmlNode di in dataItems)
					{
						switch (di.Name)
						{
							case "author":
								paramAuthor = di.InnerText;
								break;
							case "title":
								paramTitle = di.InnerText;
								break;
							case "genre":
								paramGenre = di.InnerText;
								break;
							case "price":
								paramPrice = di.InnerText;
								break;
							case "publish_date":
								paramPublishDate = di.InnerText;
								break;
							case "description":
								paramDescription = di.InnerText;
								break;
						}
					}

					books.Add(new MyBookInfo(paramId, paramAuthor, paramTitle, paramGenre,
						paramPrice, paramPublishDate, paramDescription));
				}
			}

			return books;
		}
	}

	internal class MyBookInfo : BookInfo
	{
		public MyBookInfo(string id, string title, string author, string genre, string price, string publishDate,
			string description)
		{
			Id = id;
			Title = title;
			Author = author;
			Genre = genre;
			Price = price;
			PublishDate = publishDate;
			Description = description;
		}

		public override string Id { get; }
		public override string Title { get; }
		public override string Author { get; }
		public override string Genre { get; }
		public override string Price { get; }
		public override string PublishDate { get; }
		public override string Description { get; }
	}
}