﻿using Crushy.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Crushy.Data
{

	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}

		public DbSet<User> Users { get; set; }
		public DbSet<UserProfile> UserProfiles { get; set; }
		public DbSet<BlockedUser> BlockedUsers { get; set; }
		public DbSet<MatchedUser> MatchedUsers { get; set; }
		public DbSet<Message> Messages { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);


			modelBuilder.Entity<User>().HasData(
				new User { Id=1,CreatedAt=DateTime.Now,IsDeleted=false,Password= BCrypt.Net.BCrypt.HashPassword("string"), Role= "VerifiedUser", Username ="string" }
				);
			modelBuilder.Entity<UserProfile>().HasData(
				new UserProfile { Coin= 20,Email="string@gmail.com",Fullname="string",Gender=true,UserId=1 }
				);

			// Soft delete edilen verileri filtrele
			modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
			modelBuilder.Entity<BlockedUser>().HasQueryFilter(b => !b.IsDeleted);
			modelBuilder.Entity<Message>().HasQueryFilter(m => !m.IsDeleted);


			modelBuilder.Entity<User>()
				.HasOne(u => u.Profile)
				.WithOne(p => p.User)
				.HasForeignKey<UserProfile>(p => p.UserId);


			// Bir kullanıcının başka bir kullanıcıyı engelleyebileceği ilişki
			modelBuilder.Entity<BlockedUser>()
				.HasOne(b => b.User)
				.WithMany(u => u.BlockedUsers)
				.HasForeignKey(b => b.UserId)
				.OnDelete(DeleteBehavior.Restrict); // Kullanıcı silindiğinde engellenenler silinmez

			modelBuilder.Entity<BlockedUser>()
				.HasOne(b => b.Blocked)
				.WithMany()
				.HasForeignKey(b => b.BlockedUserId)
				.OnDelete(DeleteBehavior.Restrict);

			// Mesaj ilişkisi (Gönderici ve Alıcı)
			modelBuilder.Entity<Message>()
				.HasOne(m => m.Sender)
				.WithMany(u => u.SentMessages)
				.HasForeignKey(m => m.SenderId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Message>()
				.HasOne(m => m.Receiver)
				.WithMany(u => u.ReceivedMessages)
				.HasForeignKey(m => m.ReceiverId)
				.OnDelete(DeleteBehavior.Restrict);



			modelBuilder.Entity<MatchedUser>()
				.HasOne(m => m.User1)
				.WithMany(u => u.MatchesAsUser1)
				.HasForeignKey(m => m.User1Id)
				.OnDelete(DeleteBehavior.Restrict); 

			modelBuilder.Entity<MatchedUser>()
				.HasOne(m => m.User2)
				.WithMany(u => u.MatchesAsUser2)
				.HasForeignKey(m => m.User2Id)
				.OnDelete(DeleteBehavior.Restrict);

		}

	}
}
