using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;

namespace XamarinPaint.iOS.Demo
{
    public sealed class TableViewSelector<T> : UITableViewController
    {
        private readonly List<T> _elements;
        private readonly T _preselectedElement;

        public event EventHandler<T> OnSelected; 

        public TableViewSelector(List<T> elements, T preselectedElement)
        {
            _elements = elements;
            _preselectedElement = preselectedElement;

            ModalPresentationStyle = UIModalPresentationStyle.Popover;
            PreferredContentSize = new CGSize(200.0, 300.0);
        }

        #region View life cylce

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.Source = new TableViewSelectorSource<T>(_elements);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            var source = TableView.Source as TableViewSelectorSource<T>;
            if (source != null)
            {
                source.OnSelected += SourceOnOnSelected;
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (_preselectedElement != null)
            {
                try
                {
                    var idx = _elements.IndexOf(_preselectedElement);
                    var indexPath = NSIndexPath.FromRowSection(idx, 0);
                    TableView.SelectRow(indexPath, false, UITableViewScrollPosition.Top);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            var source = TableView.Source as TableViewSelectorSource<T>;
            if (source != null)
            {
                source.OnSelected -= SourceOnOnSelected;
            }
        }

        #endregion

        #region Events

        private void SourceOnOnSelected(object sender, T element)
        {
            DismissViewController(true, null);
            OnSelected?.Invoke(this, element);
        }

        #endregion
    }

    public class TableViewSelectorSource<T> : UITableViewSource
    {
        private readonly List<T> _elements;

        public event EventHandler<T> OnSelected; 

        public TableViewSelectorSource(List<T> elements)
        {
            _elements = elements;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _elements?.Count ?? 0;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            const string reuseIdentifier = "TableViewSelectCell";
            var cell = tableView.DequeueReusableCell(reuseIdentifier) ?? new UITableViewCell(UITableViewCellStyle.Default, reuseIdentifier);

            cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;

            var element = _elements.ElementAtOrDefault(indexPath.Row);
            if (element != null)
            {
                cell.TextLabel.Text = element.ToString();
            }

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, true);

            var element = _elements.ElementAtOrDefault(indexPath.Row);
            if (element != null)
            {
                OnSelected?.Invoke(this, element);
            }
        }
    }
}