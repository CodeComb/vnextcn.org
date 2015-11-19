using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeComb.vNextChina.Models
{
	public class Tag
	{
		public Guid Id { get; set; }
		
		public long PRI { get; set; }
		
		[MaxLength(128)]
		public string Title { get; set; }
		
		[ForeignKey("Parent")]
		public Guid? ParentId { get; set; }
		
		public virtual Tag Parent { get; set; }
		
		public virtual ICollection<Tag> SubTags { get; set; }
	}
}