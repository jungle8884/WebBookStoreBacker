//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class BookRemark
    {
        public int BookRemarkId { get; set; }
        public Nullable<int> BookId { get; set; }
        public Nullable<int> UserId { get; set; }
        public string BookRemarks { get; set; }
        public string UserName { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public string ClientIP { get; set; }
    }
}
