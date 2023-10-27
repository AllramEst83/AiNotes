using AiNotes.Exstensions;
using AiNotes.Models;
using System.Globalization;

namespace AiNotes.Converters
{
    public class EnumToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Assuming you have a GetDescription extension method for enums
            return ((NoteType)value).GetDescription();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Implement if needed
            throw new NotImplementedException();
        }
    }

}
