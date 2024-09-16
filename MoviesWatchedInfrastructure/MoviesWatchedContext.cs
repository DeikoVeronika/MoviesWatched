using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MoviesWatchedDomain.Model;

public partial class MoviesWatchedContext : DbContext
{
    public MoviesWatchedContext()
    {
    }

    public MoviesWatchedContext(DbContextOptions<MoviesWatchedContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Actor> Actors { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<MoviesActor> MoviesActors { get; set; }

    public virtual DbSet<MoviesGenre> MoviesGenres { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-VLOMFU1\\SQLEXPRESS; Database=MoviesWatched; Trusted_Connection=True; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Actor>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.Property(e => e.ReviewDate).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(50);

            entity.HasOne(d => d.Language).WithMany(p => p.Movies)
                .HasForeignKey(d => d.LanguageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Movies_Languages");
        });

        modelBuilder.Entity<MoviesActor>(entity =>
        {
            entity.ToTable("MoviesActor");

            entity.HasOne(d => d.Actor).WithMany(p => p.MoviesActors)
                .HasForeignKey(d => d.ActorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MoviesActor_Actors");

            entity.HasOne(d => d.Movie).WithMany(p => p.MoviesActors)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MoviesActor_Movies");
        });

        modelBuilder.Entity<MoviesGenre>(entity =>
        {
            entity.ToTable("MoviesGenre");

            entity.HasOne(d => d.Genre).WithMany(p => p.MoviesGenres)
                .HasForeignKey(d => d.GenreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MoviesGenre_Genres");

            entity.HasOne(d => d.Movie).WithMany(p => p.MoviesGenres)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MoviesGenre_Movies");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
