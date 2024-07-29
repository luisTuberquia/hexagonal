using FDLM.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Test.Utilities
{
    public class Tools_UTest
    {
        private readonly string dateFormat = "dd-MM-yyyy HH:mm:ss";

        public Tools_UTest()
        {

        }


        [Fact]
        void Sha256Hash()
        {
            // Given
            string original = "brutos, pero decididos";
            string hashed = "78dfaf6cff129f53c31a0e054868a42a15987fd15e70ac1f99ee99dcecffb572";

            ITools tools = new Tools();

            //When
            string result = tools.SHA256Hash(original);

            //Then
            Assert.Equal(hashed, result);
        }

        [Fact]
        public void ConvertDateTimeUtcToBogota()
        {
            // Given
            DateTime myDateInUtc = DateTime.ParseExact("01-12-2024 17:00:00", dateFormat, CultureInfo.InvariantCulture);
            DateTime myDateInBogota = DateTime.ParseExact("01-12-2024 12:00:00", dateFormat, CultureInfo.InvariantCulture);

            ITools tools = new Tools();

            //When
            DateTime result = tools.DateTimeUtcToBogota(myDateInUtc);

            //Then
            Assert.Equal(myDateInBogota, result);
        }

        [Fact]
        public void ConvertDateTimeUtcToBogotaAsString()
        {
            // Given
            DateTime myDateInUtc = DateTime.ParseExact("01-12-2024 17:00:00", dateFormat, CultureInfo.InvariantCulture);
            string myDateInBogota = "01-12-2024 12:00:00";

            ITools tools = new Tools();

            //When
            string result = tools.DateTimeUtcToBogotaAsString(myDateInUtc);

            //Then
            Assert.Equal(myDateInBogota, result);
        }

        [Fact]
        public void ConvertDateTimeBogotaToUtc()
        {
            // Given
            DateTime myDateInBogota = DateTime.ParseExact("01-12-2024 12:00:00", dateFormat, CultureInfo.InvariantCulture);
            DateTime myDateInUtc = DateTime.ParseExact("01-12-2024 17:00:00", dateFormat, CultureInfo.InvariantCulture);

            ITools tools = new Tools();

            //When
            DateTime result = tools.DateTimeBogotaToUtc(myDateInBogota);

            //Then
            Assert.Equal(myDateInUtc, result);
        }

        [Fact]
        public void ConvertDateTimeBogotaToUtcAsString()
        {
            // Given
            DateTime myDateInBogota = DateTime.ParseExact("01-12-2024 12:00:00", dateFormat, CultureInfo.InvariantCulture);
            string myDateInUtc = "01-12-2024 17:00:00";

            ITools tools = new Tools();

            //When
            string result = tools.DateTimeBogotaToUtcAsString(myDateInBogota);

            //Then
            Assert.Equal(myDateInUtc, result);
        }

        [Fact]
        void DateTimeToString()
        {
            // Given
            DateTime myDate = DateTime.ParseExact("01-12-2024 17:00:00", dateFormat, CultureInfo.InvariantCulture);
            string myDateAsstring = "01-12-2024 17:00:00";

            ITools tools = new Tools();

            //When
            string result = tools.DateTimeToString(myDate);

            //Then
            Assert.Equal(myDateAsstring, result);
        }

        [Fact]
        void DateStringToDateTime()
        {
            // Given
            DateTime myDate = DateTime.ParseExact("01-12-2024 17:00:00", dateFormat, CultureInfo.InvariantCulture);
            string myDateAsstring = "01-12-2024 17:00:00";

            ITools tools = new Tools();

            //When
            DateTime result = tools.DateStringToDateTime(myDateAsstring);

            //Then
            Assert.Equal(myDate, result);
        }


        [Fact]
        public void ConvertNullstringToBogota()
        {
            // Given
            string myDateInUtc = null;
            ITools tools = new Tools();

            //When
            DateTime result = tools.DateStringToDateTime(myDateInUtc);

            //Then
            Assert.Equal(DateTime.MinValue, result);
        }

        [Fact]
        void DateTimeBogotaToUtc_NullInput()
        {
            ITools tools = new Tools();
            string nullDateTimeUtc = null;

            DateTime result = tools.DateTimeBogotaToUtc(nullDateTimeUtc);

            Assert.Equal(DateTime.MinValue, result);
        }

        [Fact]
        void DateTimeBogotaToUtcAsString_NullInput()
        {
            ITools tools = new Tools();
            string nullDateTimeUtc = null;

            string result = tools.DateTimeBogotaToUtcAsString(nullDateTimeUtc);

            Assert.Null(result);
        }
    }
}
