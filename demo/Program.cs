using System;

namespace demo {
    class Program {
        static void Main(string[] args) {

            dpz2.db.Row cache = new dpz2.db.Row();
            ycp.orm.Tables tables = new ycp.orm.Tables();
            dpz2.Mvc.XOrmForVue.Table form = new dpz2.Mvc.XOrmForVue.Table(null, tables, cache, new dpz2.Mvc.XOrmForVue.Config("https://dev.lywos.com/Orm/Xml", @"X:\temp\UI", @"X:\temp\Cache"), "Aos", "AosAuthorize");

            Console.WriteLine(form.GetHtml());
        }
    }
}
