using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yazlab1.Models
{
    public static class BookHelper
    {
        static YazlabDbEntities db = new YazlabDbEntities();
        public static DateTime GetDeliverDate(this Book book)
        {
            return (DateTime) db.Loans.Where(x => x.Book_Id == book.Book_Id).FirstOrDefault().Deliver_Date;
        }
    }
}