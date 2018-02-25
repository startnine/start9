using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.Runtime.Remoting;
using Start9.Api.Contracts;
using Start9.Host.AddInView;

namespace Start9.Host.Adapter
{
	public class BookInfoContractToViewHostAdapter : BookInfo
	{
		private readonly IBookInfoContract _contract;
		private ContractHandle _handle;

		public BookInfoContractToViewHostAdapter(IBookInfoContract contract)
		{
			_contract = contract;
			_handle = new ContractHandle(contract);
		}

		public override string Id => _contract.Id;

		public override string Author => _contract.Author;

		public override string Title => _contract.Title;

		public override string Genre => _contract.Genre;

		public override string Price => _contract.Price;

		public override string PublishDate => _contract.PublishDate;

		public override string Description => _contract.Description;

		internal IBookInfoContract GetSourceContract()
		{
			return _contract;
		}
	}

	public class BookInfoViewToContractHostAdapter : ContractBase, IBookInfoContract
	{
		private readonly BookInfo _view;

		public BookInfoViewToContractHostAdapter(BookInfo view)
		{
			_view = view;
		}

		public virtual string Id => _view.Id;

		public virtual string Author => _view.Author;

		public virtual string Title => _view.Title;

		public virtual string Genre => _view.Genre;

		public virtual string Price => _view.Price;

		public virtual string PublishDate => _view.PublishDate;

		public virtual string Description => _view.Description;

		internal BookInfo GetSourceView()
		{
			return _view;
		}
	}

	[HostAdapter]
	public class LibraryManagerContractToViewHostAdapter : LibraryManager
	{
		private readonly ILibraryManagerContract _contract;
		private ContractHandle _handle;

		public LibraryManagerContractToViewHostAdapter(ILibraryManagerContract contract)
		{
			_contract = contract;
			_handle = new ContractHandle(contract);
		}

		public override BookInfo BestSeller => BookInfoHostAdapter.ContractToViewAdapter(_contract.BestSeller);

		public override void ProcessBooks(IList<BookInfo> books)
		{
			_contract.ProcessBooks(CollectionAdapters.ToIListContract(books,
				BookInfoHostAdapter.ViewToContractAdapter,
				BookInfoHostAdapter.ContractToViewAdapter));
		}

		internal ILibraryManagerContract GetSourceContract()
		{
			return _contract;
		}

		public override string Data(string txt)
		{
			string rtxt = _contract.Data(txt);
			return rtxt;
		}
	}

	public class BookInfoHostAdapter
	{
		internal static BookInfo ContractToViewAdapter(IBookInfoContract contract)
		{
			if (!RemotingServices.IsObjectOutOfAppDomain(contract) &&
			    contract.GetType().Equals(typeof(BookInfoViewToContractHostAdapter)))
				return ((BookInfoViewToContractHostAdapter) contract).GetSourceView();
			return new BookInfoContractToViewHostAdapter(contract);
		}

		internal static IBookInfoContract ViewToContractAdapter(BookInfo view)
		{
			if (!RemotingServices.IsObjectOutOfAppDomain(view) &&
			    view.GetType().Equals(typeof(BookInfoContractToViewHostAdapter)))
				return ((BookInfoContractToViewHostAdapter) view).GetSourceContract();
			return new BookInfoViewToContractHostAdapter(view);
		}
	}
}