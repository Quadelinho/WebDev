﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using RestApiTest.Infrastructure.Data;
using System;

namespace RestApiTest.Migrations
{
    [DbContext(typeof(ForumContext))]
    [Migration("20190522215222_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("RestApiTest.Core.Models.BlogPost", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("AuthorId");

                    b.Property<string>("Content");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<DateTime?>("Modified");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("BlogPost");

                    b.HasDiscriminator<string>("Discriminator").HasValue("BlogPost");
                });

            modelBuilder.Entity("RestApiTest.Core.Models.Comment", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Approved");

                    b.Property<long?>("AuthorId");

                    b.Property<long?>("BlogPostId");

                    b.Property<long?>("CommentId");

                    b.Property<string>("Content");

                    b.Property<bool>("IsAdministrativeNote");

                    b.Property<bool>("IsRecommendedSolution");

                    b.Property<DateTime?>("Modified");

                    b.Property<long>("Points");

                    b.Property<DateTime>("SentDate");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("BlogPostId");

                    b.HasIndex("CommentId");

                    b.ToTable("PostComments");
                });

            modelBuilder.Entity("RestApiTest.Core.Models.ForumUser", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email");

                    b.Property<bool>("IsConfirmed");

                    b.Property<DateTime>("LastLoggedIn");

                    b.Property<string>("Login");

                    b.Property<string>("Name");

                    b.Property<DateTime>("RegisteredSince");

                    b.Property<int>("ReputationPoints");

                    b.Property<int>("Role");

                    b.Property<bool>("SubscribedToNewsletter");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("RestApiTest.Core.Models.Tag", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.Property<long?>("QuestionPostId");

                    b.HasKey("Id");

                    b.HasIndex("QuestionPostId");

                    b.ToTable("Tag");
                });

            modelBuilder.Entity("RestApiTest.Core.Models.Vote", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsLike");

                    b.Property<DateTime>("Modified");

                    b.Property<long?>("VotedCommentId");

                    b.Property<long?>("VotedPostId");

                    b.Property<long?>("VoterId");

                    b.HasKey("Id");

                    b.HasIndex("VotedCommentId");

                    b.HasIndex("VotedPostId");

                    b.HasIndex("VoterId");

                    b.ToTable("Vote");
                });

            modelBuilder.Entity("RestApiTest.Core.Models.QuestionPost", b =>
                {
                    b.HasBaseType("RestApiTest.Core.Models.BlogPost");

                    b.Property<bool?>("Approved");

                    b.Property<bool?>("IsSolved");

                    b.HasDiscriminator().HasValue("QuestionPost");
                });

            modelBuilder.Entity("RestApiTest.Core.Models.BlogPost", b =>
                {
                    b.HasOne("RestApiTest.Core.Models.ForumUser", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId");
                });

            modelBuilder.Entity("RestApiTest.Core.Models.Comment", b =>
                {
                    b.HasOne("RestApiTest.Core.Models.ForumUser", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId");

                    b.HasOne("RestApiTest.Core.Models.BlogPost")
                        .WithMany("Comments")
                        .HasForeignKey("BlogPostId");

                    b.HasOne("RestApiTest.Core.Models.Comment")
                        .WithMany("Responses")
                        .HasForeignKey("CommentId");
                });

            modelBuilder.Entity("RestApiTest.Core.Models.Tag", b =>
                {
                    b.HasOne("RestApiTest.Core.Models.QuestionPost")
                        .WithMany("Tags")
                        .HasForeignKey("QuestionPostId");
                });

            modelBuilder.Entity("RestApiTest.Core.Models.Vote", b =>
                {
                    b.HasOne("RestApiTest.Core.Models.Comment", "VotedComment")
                        .WithMany()
                        .HasForeignKey("VotedCommentId");

                    b.HasOne("RestApiTest.Core.Models.BlogPost", "VotedPost")
                        .WithMany("Votes")
                        .HasForeignKey("VotedPostId");

                    b.HasOne("RestApiTest.Core.Models.ForumUser", "Voter")
                        .WithMany()
                        .HasForeignKey("VoterId");
                });
#pragma warning restore 612, 618
        }
    }
}