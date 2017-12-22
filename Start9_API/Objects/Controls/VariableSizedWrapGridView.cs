using System.Windows;
using System.Windows.Controls;

namespace Start9.Api.Objects.Controls
{
    public class VariableSizedWrapGridView : ItemsControl
    {
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            dynamic model = item;
            try
            {
                element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, model.ColSpan);
                element.SetValue(VariableSizedWrapGrid.RowSpanProperty, model.RowSpan);
            }
            catch
            {
                element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 1);
                element.SetValue(VariableSizedWrapGrid.RowSpanProperty, 1);
            }
            finally
            {
                element.SetValue(VerticalContentAlignmentProperty, VerticalAlignment.Stretch);
                element.SetValue(HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch);
                base.PrepareContainerForItemOverride(element, item);
            }
        }
    }
}
