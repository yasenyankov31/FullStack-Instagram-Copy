using System;
using System.Collections.Generic;
using InstagramCopyApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InstagramCopyApi.Data;

public partial class InstagramContext : DbContext
{
    public InstagramContext()
    {
    }

    public InstagramContext(DbContextOptions<InstagramContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=Instagram;Integrated Security=True;Encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>();

        modelBuilder.Entity<Like>();

        modelBuilder.Entity<Post>();

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
