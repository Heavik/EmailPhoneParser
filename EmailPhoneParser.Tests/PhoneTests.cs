using Xunit;

namespace EmailPhoneParser.Tests
{
    public class PhoneTests
    {
        private const string TextSample = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor ,111-222-3333 incididunt ut labore et dolore magna aliqua. (023)     222-6666, Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur 123-654-3489.. Excepteur 101-778-9021 sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est (068) 309-3265 laborum.";

        private const string ExpectedTextSample = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor ,<a href=\"tel:111-222-3333\">111-222-3333</a> incididunt ut labore et dolore magna aliqua. <a href=\"tel:(023) 222-6666\">(023) 222-6666</a>, Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur <a href=\"tel:123-654-3489\">123-654-3489</a>.. Excepteur <a href=\"tel:101-778-9021\">101-778-9021</a> sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est <a href=\"tel:(068) 309-3265\">(068) 309-3265</a> laborum.";

        [Fact]
        public void TestFormatTextPhones()
        {
            var parser = new Parser(TextSample);
            string result = parser.FormatText();
            Assert.Equal(ExpectedTextSample, result);
        }
    }
}