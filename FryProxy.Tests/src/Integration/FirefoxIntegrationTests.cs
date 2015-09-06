using OpenQA.Selenium;

namespace FryProxy.Tests.Integration {

    public class FirefoxIntegrationTests : CommonIntegrationTests {

        protected override IWebDriver CreateDriver(Proxy proxy) {
            return CreateFirefoxDriver(proxy);
        }

    }

}