using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InstagramCopyApi.Models;

public partial class Post
{
    [Key]
    public int Id { get; set; }

    public int CreatorId { get; set; }

    public int Likes { get; set; }

    [StringLength(50)]
    public string Url { get; set; } = null!;

    public int Comments { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreationDate { get; set; }

    [StringLength(50)]
    public string? Description { get; set; }

    [NotMapped]
    public IFormFile? Image { get; set; }

}
