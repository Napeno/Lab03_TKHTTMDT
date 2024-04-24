using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using CsvHelper;
using System.IO;
using OpenQA.Selenium.Support.UI;


namespace TikiCrawler
{
    class Program
    {
        public class Value
        {
            public string ecommerceLink;
            public string itemCard;
            public string httpsLink;
            public string title;
            public string httpVn;
            public string brand;
            public string price;
            public string imgUrl;
            public string imgUrl2;
            public string hyperUrl;
            public string mrlGlass;
            public string sizeFace;

            public Value(string ecommerceLink, string itemCard, string httpsLink, string title, string httpVn, string brand, string price, string imgUrl,  string hyperUrl, string imgUrl2, string mrlGlass, string sizeFace)
            {
                this.ecommerceLink = ecommerceLink;
                this.itemCard = itemCard;
                this.httpsLink = httpsLink;
                this.title = title;
                this.httpVn = httpVn;
                this.brand = brand;
                this.price = price;
                this.imgUrl = imgUrl;
                this.hyperUrl = hyperUrl;
                this.imgUrl2 = imgUrl2;
                this.mrlGlass = mrlGlass;
                this.sizeFace = sizeFace;
            }

            public class Product
            {
                public string? Name { get; set; }
                public string? Price { get; set; }
                public string? Brand { get; set; }
                public string? Img { get; set; }
                public string? Img2 { get; set; }
                public string? materialGlass { get; set; }
                public string? sizeFace { get; set; }

            }


            static void Main(string[] args)
            {

                //Create an instance of Chrome driver
                IWebDriver browser = new ChromeDriver();

                var listProducts = new List<Product>();

               // var tikiValue = new Value
               //("https://tiki.vn/laptop/c8095",
               //".product-item",
               //"https://",
               //"title",
               //"https://tiki.vn",
               //"a[data-view-id='pdp_details_view_brand']",
               //".product-price__current-price",
               //"//img[@alt='product-img-0']",
               //null,
               //null);

                

                var watchValue = new Value
                ("https://www.watchstore.vn/collections/dong-ho-nam-chinh-hang",
                ".item.ent-card",
                "",
                "title",
                "",
                ".specnamenew a",
                ".product__price.on-sale",
                "div[data-tab='3'] > div.item-border > img",
                "#o=6&q=",
                "div[data-tab='2'] > div.item-border > img",
                "tbody tr:nth-child(2) td:nth-child(2)",
                "tbody tr:nth-child(4) td:nth-child(2)")
                ;

                var watchValue2 = new Value
                ("https://www.dangquangwatch.vn/sp/t-1/Dong-ho-nam.html",
                ".itemPro",
                "",
                "div[class='detail_top_right_left'] h1 strong",
                "https://dangquangwatch.vn",
                "div.product-detail_way a:nth-of-type(3)",
                "div.detail_price > span.price",
                "div.owl-stage > div.owl-item:nth-child(1) > div.item.wImage > div.image > img",
                "?&page=",
                "div.owl-stage > div.owl-item:nth-child(2) > div.item.wImage > div.image > img",
                "div[class='detail_product_right'] div:nth-child(3) p:nth-child(2)",
                "div[id='product'] div:nth-child(1) p:nth-child(2) a:nth-child(1)");

                List<Value> valueList = new List<Value>();
                //valueList.Add(tikiValue);
                valueList.Add(watchValue);
                valueList.Add(watchValue2);


                for (int j = 0; j < valueList.Count; j++)
                {
                    List<string> listProductLink = new List<string>();
                    for (int i = 1; i < 40; i++)
                    {
                        //Navigate to website Tiki.vn > Laptop category
                        browser.Navigate().GoToUrl(valueList[j].ecommerceLink + valueList[j].hyperUrl + i);
                        IJavaScriptExecutor js = (IJavaScriptExecutor)browser;

                        //Select all product items by CSS Selector

                        var products = browser.FindElements(By.CssSelector(valueList[j].itemCard));
                        foreach (var product in products)
                        {
                            string outerHtml = product.GetAttribute("outerHTML");
                            string productLink = Regex.Match(outerHtml, "href=\"(.*?)\"").Groups[1].Value;
                            //productLink = "https://" + productLink;
                            listProductLink.Add(productLink);
                        }
                        IWebElement idButton;
                        if (valueList[j].itemCard == ".item.ent-card")
                        {
                            try
                            {
                                
                                idButton = browser.FindElement(By.CssSelector("a.paged_ul_li[data-id='" + i + "']"));
                            }
                            catch (StaleElementReferenceException)
                            {
                                
                                idButton = browser.FindElement(By.CssSelector("a.paged_ul_li[data-id='" + i + "']"));
                            }

                        }
                        else if(valueList[j].itemCard == ".itemPro")
                        {
                            try {
                                i++;
                                idButton = browser.FindElement(By.CssSelector("a[href='/sp/t-1/Dong-ho-nam.html?&page=" + i + "']"));
                           
                            }
                            catch (StaleElementReferenceException)
                            {
                                i++;
                                idButton = browser.FindElement(By.CssSelector("a[href='/sp/t-1/Dong-ho-nam.html?&page=" + i + "']"));
                            }
                        }
                        else
                        {
                            break;
                        }
                        idButton.Click();
                        System.Threading.Thread.Sleep(1200);
                    }
                    




                    //Go to each product link
                    for (int i = 0; i < listProductLink.Count - 1; i++)
                    {
                        Console.WriteLine("DEBUG: " + listProductLink[i].ToString());

                        //Go to product link
                        try
                        {
                            browser.Navigate().GoToUrl(valueList[j].httpsLink + listProductLink[i]);
                        }
                        catch
                        {
                            browser.Navigate().GoToUrl(valueList[j].httpVn + listProductLink[i]);
                        }

                        IWebElement titleElement = browser.FindElement(By.CssSelector(valueList[j].title));
                        IWebElement brandElement = browser.FindElement(By.CssSelector(valueList[j].brand));
                        IWebElement priceElement = browser.FindElement(By.CssSelector(valueList[j].price));
                        IWebElement image1Element = browser.FindElement(By.CssSelector(valueList[j].imgUrl));
                        IWebElement image1Element2 = null;
                        try
                        {
                            image1Element2 = browser.FindElement(By.CssSelector(valueList[j].imgUrl2));
                        }
                        catch (NoSuchElementException)
                        {
                            image1Element2 = null;
                        }
                        IWebElement mrlGlassElement = browser.FindElement(By.CssSelector(valueList[j].mrlGlass));
                        IWebElement sizeFaceElement = browser.FindElement(By.CssSelector(valueList[j].sizeFace));


                        string productTitle = titleElement.GetAttribute("textContent");
                        string productBrand = brandElement.GetAttribute("textContent");
                        string productPrice = priceElement.GetAttribute("textContent");
                        string productImg = image1Element.GetAttribute("src");
                        string productImg2 = null;
                        if (image1Element2 != null)
                        {
                            productImg2 = image1Element2.GetAttribute("src");
                        }
                        string productmrlGlass = mrlGlassElement.GetAttribute("textContent");
                        string sizeFace = sizeFaceElement.GetAttribute("textContent");


                        var productList = new Product { Name = productTitle, Brand = productBrand, Price = productPrice, Img = productImg, Img2 = productImg2 , materialGlass = productmrlGlass, sizeFace = sizeFace };
                        listProducts.Add(productList);

                        Console.WriteLine("DEBUG TITLE: " + productTitle);
                        //Extract product brand by CSS Selector then remove redundant data by Regular Expression
                        //string productBrand = browser.FindElements(By.CssSelector(".brand-and-autho
                        //string productTitle = browser.FindElements(By.CssSelector(".title"))[0].Tr"))[0].GetAttribute("outerHTML");
                        //string productBrand = browser.FindElements(By.XPath("//a[@data-view-id='pdp_details_view_brand']"))[0].Text;
                        //productBrand = Regex.Match(productBrand, "brand\">(.*?)</a>").Groups[1].Value;
                        Console.WriteLine("DEBUG BRAND: " + productBrand);
                        //Extract product price
                        //string productPrice = browser.FindElements(By.CssSelector(".product-price__current-price"))[0].Text;
                        productPrice = Regex.Match(productPrice, "^[\\d|\\.|\\,]+").Value;
                        Console.WriteLine("DEBUG PRICE: " + productPrice);

                        Console.WriteLine("DEBUG IMG1: " + productImg);
                        Console.WriteLine("DEBUG IMG1: " + productImg2);
                        Console.WriteLine("DEBUG Material Glass: " + productmrlGlass);
                        Console.WriteLine("DEBUG Size Face: " + sizeFace);
                        //Console.WriteLine("DEBUG IMG2: " + productImg2);
                        //Console.WriteLine("DEBUG IMG3: " + productImg3);


                        //Extract product images

                        //Extract colors

                        //Extract sizes

                        //Extract product details

                        //Extract product description

                        //System.Threading.Thread.Sleep(300);
                    }

                    //Console.WriteLine(products.Count);
                    //System.IO.StreamWriter writer = new System.IO.StreamWriter("D:\\tiki.csv", false, System.Text.Encoding.UTF8);
                    //writer.WriteLine("ProductName\tImageLink");
                    ////System.Threading.Thread.Sleep(10000);
                    ////string productLink = product.GetAttribute("href");
                    ////string productName = product.FindElement(By.CssSelector(".product-item .name")).Text;
                    ////string innerHtml = product.GetAttribute("innerHTML");
                    //string productName = Regex.Match(outerHtml, "alt=\"(.*?)\"").Groups[1].Value;
                    //string productThumbnail = Regex.Match(outerHtml, "<img src=\"(.*?)\"").Groups[1].Value;
                    //writer.WriteLine(productName + "\t" + productThumbnail);
                    //writer.Close();

                    //browser.FindElements(By.CssSelector(".title"))[0].Text;
                    //browser.FindElements(By.CssSelector(".title"))[0].GetAttribute("");
                }

                // create the CSV output file
                using (var writer = new StreamWriter("products.csv"))
                // instantiate the CSV writer
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    // populate the CSV file
                    csv.WriteRecords(listProducts);
                }


            }
        }
    }
}

//browser.FindElements(By.XPath(""));
//browser.FindElement(By.CssSelector(""));
//browser.FindElement(By.XPath(""));