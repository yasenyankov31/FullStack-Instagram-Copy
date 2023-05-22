using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InstagramCopyApi.Models;

public partial class Comment
{
    [Key]
    public int Id { get; set; }

    public int? PostId { get; set; }

    public int? CommentatorId { get; set; }

    [StringLength(100)]
    public string? CommentContent { get; set; }

    public int? Likes { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateCreated { get; set; }
}
