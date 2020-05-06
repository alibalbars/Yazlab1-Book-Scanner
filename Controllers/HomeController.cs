using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tesseract;
using Yazlab1.Models;
using System.Text.RegularExpressions;
using System.Text;

namespace Yazlab1.Controllers
{
    public class HomeController : Controller
    {
        YazlabDbEntities db;
        DateTime currentDateTime;
        string currentDateText;

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public void PostUsingParameters(string name, string pass)
        {
            db = new YazlabDbEntities();
            User currentUser;
            if (name.ToLower() == "admin")
            {
                Response.Redirect("AdminPage");
            }
            else
            {
                try
                {
                    currentUser = db.Users.Where(x => x.First_Name.ToLower().Trim() == name.ToLower().Trim())
                    .FirstOrDefault();
                    MakeCurrentUser(currentUser.User_Id);
                }
                catch (Exception e)
                {
                    throw new HttpException("Kullanıcı bulunamadı.");
                }
                Response.Redirect("UserPage");
            }
        }

        public ActionResult AdminPage()
        {
            GetGreatestDate();
            ViewBag.currentDateText = GetGreatestDate().ToString("dd/MM/yyyy");
            //ViewBag.currentDateText = "01/01/2020";
            return View();
        }

        public void ShiftTime()
        {
            int dayNumber = 20;
            db = new YazlabDbEntities();

            AddNewTimeLog();

            currentDateTime = GetGreatestDate();
            currentDateTime = currentDateTime.AddDays(dayNumber);
            currentDateText = currentDateTime.ToString("dd/MM/yyyy");

            Response.Redirect("AdminPage");
        }

        public DateTime GetGreatestDate()
        {
            db = new YazlabDbEntities();
            DateShiftLog greatestLog = null;
            try
            {
                greatestLog = db.DateShiftLogs.OrderByDescending(x => x.Date).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw new HttpException("Database den guncel log alınamadı.");
            }

            return (DateTime)greatestLog.Date;
        }

        public void AddNewTimeLog()
        {
            db = new YazlabDbEntities();

            DateShiftLog newLog = new DateShiftLog();
            if (currentDateTime != null)
            {
                newLog.Date = GetGreatestDate().AddDays(20);
            }

            try
            {
                db.DateShiftLogs.Add(newLog);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw new HttpException("Databaseye newLog eklenemedi.");
            }

        }

        public ActionResult UserPage()
        {
            ViewBag.userName = GetCurrentUser().First_Name + " " + GetCurrentUser().Last_Name;
            return View();
        }

        public ActionResult AddNewBook()
        {
            return View();
        }

        [HttpPost]
        public string PostAddNewBook(string bookName, HttpPostedFileBase file)
        {
            if (bookName == string.Empty)
                throw new HttpException("Kitap adı girilmedi.");

            if (file == null)
                throw new HttpException("Dosya seçilmedi.");

            string text = ReadImage(file);

            string state = "Durum belirlenmedi.";
            try
            {
                Book newBook = new Book();
                newBook.Book_Name = bookName;
                newBook.Book_Isbn = text;

                db.Books.Add(newBook);
                db.SaveChanges();
                state = "Kitap kaydedildi.";
            }
            catch (Exception e)
            {
                state = "Kitap kaydedilemedi." + "<br>" + e.Message + "<br><br>" + e.StackTrace
                    + "<br><br>" + "ISBN uzunluk: " + text.Length + "<br><br>" + "Trim uzunluk: " + text.Length;
            }
            return "Kitap adı: " + bookName + "<br><br>" + "ISBN: " + text
                + "<br><br>" + state;
        }

        public ActionResult ListBooks(string bookNameSearch, string IsbnSearch)
        {
            db = new YazlabDbEntities();
            List<Book> books = new List<Book>();
            Book searchedBook;

            if (bookNameSearch != null)
            {
                searchedBook = db.Books.Where(x => x.Book_Name.ToLower().Trim() == bookNameSearch.Trim())
                    .FirstOrDefault();
                
                if (searchedBook != null)
                {
                    MakeCurrentBook(searchedBook.Book_Id);
                    books.Clear();
                    books.Add(searchedBook);
                }
                
            }
            else if (IsbnSearch != null)
            {
                searchedBook = db.Books.Where(x => x.Book_Isbn == IsbnSearch)
                    .FirstOrDefault();
                
                if (searchedBook != null)
                {
                    MakeCurrentBook(searchedBook.Book_Id);
                    books.Clear();
                    books.Add(searchedBook);
                }
            }
            else
            {
                books = db.Books.ToList();
            }
            ViewBag.books = books;
            return View();
        }

        public string LoanBook()
        {
            int bookId = GetCurrentBook().Book_Id;
            int userId = GetCurrentUser().User_Id;
            db = new YazlabDbEntities();
            Loan loan = new Loan();

            //List'in item sayısı 0'dan büyükse
            if (GetUserExpiredBookList(userId).Count > 0)
            {
                var userExpiredBookList = GetUserExpiredBookList(userId);
                //Teslim tarihi geçen kitap isimlerini alma
                StringBuilder sb = new StringBuilder();
                sb.Append("Teslim tarihi geçen kitaplar: ");
                sb.Append(Environment.NewLine);
                userExpiredBookList.ForEach(x => sb.Append(x.Book_Name + " "));

                return "Teslim alma başarısız!<br>"+ GetCurrentUser().First_Name + " " + GetCurrentUser().Last_Name + 
                    ", teslim tarihi geçmiş kitaplara sahip.<br><br>" + sb.ToString();
            }
            //Kullanımda ise
            else if (!IsBookSuitable(bookId))
            {
                return "Teslim alma başarısız!<br><br>Kitap kullanımda olduğu için alınamadı!";
            }
            //Halihazırda 3 kitabı varsa
            else if (CheckUserSuitability(GetCurrentUser()) == 1)
            {
                return "Teslim alma başarısız!<br><br>" + GetCurrentUser().First_Name + " " + GetCurrentUser().Last_Name
                    + ", halihazırda 3 kitaba sahip <br> Yeni kitap almak için kitaplarından birini teslim et.";
            }
            else
            {
                loan.Book_Id = bookId;
                loan.User_Id = userId;
                loan.Deliver_Date = GetGreatestDate().AddDays(7);
                db.Loans.Add(loan);
                db.SaveChanges();

                return "Teslim alma başarılı! <br><br>" + GetCurrentUser().First_Name + " " + GetCurrentUser().Last_Name
                    + ", " + GetCurrentBook().Book_Name + " adlı kitabı başarıyla aldı.";
            }

        }

        public void MakeCurrentBook(int bookId)
        {
            Book currentBook;
            db = new YazlabDbEntities();
            var allBookList = db.Books.ToList();

            foreach (var book in allBookList)
            {
                book.IsCurrent = false;
            }

            currentBook = db.Books.Where(x => x.Book_Id == bookId).FirstOrDefault();
            currentBook.IsCurrent = true;
            db.SaveChanges();
        }

        public void MakeCurrentUser(int userId)
        {
            User currentUser;
            db = new YazlabDbEntities();
            var allUserList = db.Users.ToList();

            foreach (var user in allUserList)
            {
                user.IsCurrent = false;
            }

            currentUser = db.Users.Where(x => x.User_Id == userId).FirstOrDefault();
            currentUser.IsCurrent = true;
            db.SaveChanges();
        }

        public User GetCurrentUser()
        {
            User currentUser;
            db = new YazlabDbEntities();

            currentUser = db.Users.Where(x => x.IsCurrent == true).FirstOrDefault();
            return currentUser;
        }

        public Book GetCurrentBook()
        {
            Book currentBook;
            db = new YazlabDbEntities();

            currentBook = db.Books.Where(x => x.IsCurrent == true).FirstOrDefault();
            return currentBook;
        }

        public int CheckUserSuitability(User user)
        {
            db = new YazlabDbEntities();
            int loanCount;
            loanCount = db.Loans.Where(x => x.User_Id == user.User_Id).ToList().Count;

            if (loanCount > 2)
            {
                return 1;
            }
            return 0;
        }

        public Boolean IsBookSuitable(int bookId)
        {
            db = new YazlabDbEntities();
            return (db.Loans.Where(x => x.Book_Id == bookId).ToList().Count) > 0 ? false : true;
            //true => kitap müsait
            //false => kitap kullanımda
        }

        public List<Book> GetUserExpiredBookList(int userId)
        {
            //null kontrolü
            if (userId == 0) return new List<Book>();

            db = new YazlabDbEntities();
            List<Book> userExpiredBookList = new List<Book>();
            var userBookList = GetUsersBook(userId);
            currentDateTime = GetGreatestDate();

            var allExpireBookIdList = db.Loans.Where(x => x.Deliver_Date < currentDateTime)
                .Select(x => x.Book_Id).ToList();

            foreach (var book in userBookList)
            {
                if (allExpireBookIdList.Contains(book.Book_Id))
                    userExpiredBookList.Add(book);
            }

            return userExpiredBookList;
        }

        public ActionResult MyBooks()
        {
            db = new YazlabDbEntities();
            User currentUser = GetCurrentUser();
            ViewBag.userName = currentUser.First_Name + " " + currentUser.Last_Name;
            List<Book> books = GetUsersBook(currentUser.User_Id);
            List<string> deliverDateTextList = new List<string>();
            List<DateTime> deliverDateTimeList = new List<DateTime>();

            books.ForEach(delegate(Book book)
            {
                deliverDateTextList.Add(book.GetDeliverDate().ToString("dd/MM/yyyy"));
                deliverDateTimeList.Add(book.GetDeliverDate());
            });

            if (GetCurrentUser() != null)
            {
                try
                { 
                    ViewBag.books = books;
                    ViewBag.deliverDateTextList = deliverDateTextList;
                    ViewBag.deliverDateTimeList = deliverDateTimeList;
                }
                catch (Exception e)
                {
                    throw new HttpException(e.Message);
                }

            }

            ViewBag.currentDateTime = GetGreatestDate();
            ViewBag.currentDateText = GetGreatestDate().ToString("dd/MM/yyyy");
            return View();
        }

        public ActionResult GiveBook(HttpPostedFileBase file, string isbnByHand)
        {
            Book book;
            string resultIsbn = string.Empty;
            db = new YazlabDbEntities();
            ViewBag.userName = GetCurrentUser().First_Name + " " + GetCurrentUser().Last_Name;
            string deliverState;

            //if (isbnByHand != string.Empty || isbnByHand != null || )
            //{
            //    resultIsbn = isbnByHand;
            //}
            if (file != null)
            {
                resultIsbn = ReadImage(file);
            }

            if (/*resultIsbn != string.Empty || */resultIsbn != null)
            {
                book = db.Books.Where(x => x.Book_Isbn == resultIsbn).FirstOrDefault();
                if (book != null)
                {
                    deliverState = DeliverBook(book);
                    ViewBag.deliverState = deliverState;
                }
            }
           
            return View();
        }

        public string ReadImage(HttpPostedFileBase file)
        {
            Bitmap bitmap;
            TesseractEngine engine;
            Page page;
            string text, fileName, path;
            db = new YazlabDbEntities();


            fileName = Path.GetFileName(file.FileName);
            path = Path.Combine(Server.MapPath("~/Kitap_Fotograflari/"), fileName);
            bitmap = new Bitmap(path);
            engine = new TesseractEngine(@"C:\Users\ASUS\Documents\Okul\Yazılım Laboratuvarı\Proje 1\Yazlab1\packages\Tesseract.3.3.0\tessdata", "eng");
            page = engine.Process(bitmap, PageSegMode.Auto);
            text = page.GetText();

            string textCorrect = EditStringAccordingFirstChars(EditStringForIsbn(text), "978");

            return textCorrect;
        }

        public string EditStringForIsbn(string inputStr)
        {
            if (inputStr == null || inputStr == string.Empty)
                return string.Empty;

            char[] array = inputStr.ToCharArray();
            array = array.Where(x => x != '\n').ToArray();
            array = array.Where(x => x != ' ').ToArray();
            array = array.Where(x => x != '|').ToArray();
            array = array.Where(x => x != '>').ToArray();

            var list = new List<char>(array);

            //Number değilse listeden çıkar
            for (int i = 0; i < list.Count(); i++)
            {
                if (!Char.IsDigit(list[i]))
                {
                    list.Remove(list[i]);
                }
            }

            //System.InvalidOperationException, Koleksiyon değiştirildi; 
            //sabit listesi işlemi yürütülemeyebilir.
            //list.ForEach(x =>
            //{
            //    if (Char.IsNumber(x))
            //        list.Remove(x);
            //});

            array = list.ToArray();

            return new string(array);
        }

        public string EditStringAccordingFirstChars(string inputStr, string firstChars)
        {
            if (inputStr == null || inputStr == string.Empty ||
                firstChars == null || firstChars == string.Empty)
                return string.Empty;

            char[] arrayStr = inputStr.ToCharArray();
            char[] arrayFirstChars = firstChars.ToCharArray();
            bool[] checkChars = new bool[arrayFirstChars.Count()];

            StringBuilder editedSb = new StringBuilder();
            bool flag = false;
            bool isAddable = false;

            //ArrayIndexOfBound Hatası almamak için
            for (int i = 0; i < arrayStr.Count() - arrayFirstChars.Count(); i++)
            {
                if (arrayStr[i] == arrayFirstChars[0])
                    if (arrayStr[i+1] == arrayFirstChars[1])
                        if (arrayStr[i+2] == arrayFirstChars[2])
                        {
                            //978 bulunduysa, i = 9'un indexi

                            //ilk 978 ise
                            if(flag == false)
                                isAddable = true;
                            else
                                isAddable = false;
                            
                            flag = true;
                        }

                if (isAddable)
                {
                    editedSb.Append(arrayStr[i].ToString());
                }

            }
            return editedSb.ToString();
        }

        public string DeliverBook(Book book)
        {
            db = new YazlabDbEntities();
            Loan loan;
            string deliverState;
            if (DoesCurrentUserHave(book))
            {
                loan = db.Loans.Where(x => x.Book_Id == book.Book_Id).FirstOrDefault();
                db.Loans.Remove(loan);
                db.SaveChanges();
                return "Kitap teslim edildi.";
            }
            else
            {
                return "Teslim edilemedi. " + GetCurrentUser().First_Name + " " + GetCurrentUser().Last_Name
                    + " zaten " + book.Book_Name + " adlı kitaba sahip değil.";
            }
        }

        public Boolean DoesCurrentUserHave(Book book)
        {
            db = new YazlabDbEntities();
            User user = GetCurrentUser();
            int bookId = book.Book_Id;
            List<Loan> usersLoans = db.Loans.Where(x => x.User_Id == user.User_Id).ToList();

            foreach (var loan in usersLoans)
            {
                if (loan.Book_Id == book.Book_Id)
                {
                    return true;
                }
            }
            
            return false;
        }

        public ActionResult ListUsers()
        {
            db = new YazlabDbEntities();
            var allUserList = db.Users.ToList();
            ViewBag.users = allUserList;
            List<string> userNames = new List<string>();

            //Her kullanıcı için, kullanıcının tüm kitapları (foreach)
            foreach (var user in allUserList)
            {
                userNames.Add(user.First_Name + " " + user.Last_Name);
            }
            //Kitap silince Loans'dan o kitabın kaydını silmeyi unutma.

            List<UserHelper> userHelperList = new List<UserHelper>();

            foreach (User user in allUserList)
            {
                UserHelper newUserHelper = new UserHelper();
                newUserHelper.user = user;
                newUserHelper.bookList = GetUsersBook(user.User_Id);
                userHelperList.Add(newUserHelper);
            }

            ViewBag.UserHelperList = userHelperList;
            return View();
        }

        public List<Book> GetUsersBook(int userId)
        {
            db = new YazlabDbEntities();
            List<Book> usersBooks = new List<Book>();

            try
            {
                List<Loan> usersLoans = db.Loans.Where(x => x.User_Id == userId).ToList();

                foreach (var loan in usersLoans)
                {
                    //Where içinde metod çağırınca hata veriyor (Query'ye dönüştürme hatası).
                    usersBooks.Add(db.Books.Where(x => x.Book_Id == loan.Book_Id).FirstOrDefault());
                }
            }
            catch (Exception e)
            {
                throw new HttpException(e.Message);
            }
            return usersBooks;
        }

    }
}