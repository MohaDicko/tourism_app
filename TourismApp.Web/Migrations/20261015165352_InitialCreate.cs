using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourismApp.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Endroits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Numero = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Statut = table.Column<int>(type: "INTEGER", nullable: false),
                    Latitude = table.Column<double>(type: "REAL", nullable: false),
                    Longitude = table.Column<double>(type: "REAL", nullable: false),
                    TypeCategorie = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    AudioUrl = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    JardinType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    MonumentDateCreation = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    RestaurantMenu = table.Column<string>(type: "TEXT", nullable: true),
                    RestaurantType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    HebergementNombreEtoile = table.Column<int>(type: "INTEGER", nullable: true),
                    HebergementType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CafeDateOuvert = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CafeDateFermer = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Endroits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Parcours",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Duree = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Distance = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parcours", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Utilisateurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    First_Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Last_Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Telephone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Nationalite = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Role = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DateInscription = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilisateurs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Evenements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    DateDebut = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateFin = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Contacte = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    EndroitId = table.Column<int>(type: "INTEGER", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evenements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evenements_Endroits_EndroitId",
                        column: x => x.EndroitId,
                        principalTable: "Endroits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ParcoursEndroits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParcoursId = table.Column<int>(type: "INTEGER", nullable: false),
                    EndroitId = table.Column<int>(type: "INTEGER", nullable: false),
                    Rang = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParcoursEndroits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParcoursEndroits_Endroits_EndroitId",
                        column: x => x.EndroitId,
                        principalTable: "Endroits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParcoursEndroits_Parcours_ParcoursId",
                        column: x => x.ParcoursId,
                        principalTable: "Parcours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Favoris",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date_ajout = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UtilisateurId = table.Column<int>(type: "INTEGER", nullable: false),
                    EndroitId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favoris", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Favoris_Endroits_EndroitId",
                        column: x => x.EndroitId,
                        principalTable: "Endroits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favoris_Utilisateurs_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NoteEndroits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Note = table.Column<int>(type: "INTEGER", nullable: false),
                    Commentaire = table.Column<string>(type: "TEXT", nullable: true),
                    DateNote = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UtilisateurId = table.Column<int>(type: "INTEGER", nullable: false),
                    EndroitId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoteEndroits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NoteEndroits_Endroits_EndroitId",
                        column: x => x.EndroitId,
                        principalTable: "Endroits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NoteEndroits_Utilisateurs_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParticiperEvenements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date_part = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UtilisateurId = table.Column<int>(type: "INTEGER", nullable: false),
                    EvenementId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticiperEvenements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParticiperEvenements_Evenements_EvenementId",
                        column: x => x.EvenementId,
                        principalTable: "Evenements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParticiperEvenements_Utilisateurs_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Evenements_EndroitId",
                table: "Evenements",
                column: "EndroitId");

            migrationBuilder.CreateIndex(
                name: "IX_Favoris_EndroitId",
                table: "Favoris",
                column: "EndroitId");

            migrationBuilder.CreateIndex(
                name: "IX_Favoris_UtilisateurId",
                table: "Favoris",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_NoteEndroits_EndroitId",
                table: "NoteEndroits",
                column: "EndroitId");

            migrationBuilder.CreateIndex(
                name: "IX_NoteEndroits_UtilisateurId",
                table: "NoteEndroits",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_ParcoursEndroits_EndroitId",
                table: "ParcoursEndroits",
                column: "EndroitId");

            migrationBuilder.CreateIndex(
                name: "IX_ParcoursEndroits_ParcoursId",
                table: "ParcoursEndroits",
                column: "ParcoursId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticiperEvenements_EvenementId",
                table: "ParticiperEvenements",
                column: "EvenementId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticiperEvenements_UtilisateurId",
                table: "ParticiperEvenements",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_Email",
                table: "Utilisateurs",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_Username",
                table: "Utilisateurs",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Favoris");

            migrationBuilder.DropTable(
                name: "NoteEndroits");

            migrationBuilder.DropTable(
                name: "ParcoursEndroits");

            migrationBuilder.DropTable(
                name: "ParticiperEvenements");

            migrationBuilder.DropTable(
                name: "Parcours");

            migrationBuilder.DropTable(
                name: "Evenements");

            migrationBuilder.DropTable(
                name: "Utilisateurs");

            migrationBuilder.DropTable(
                name: "Endroits");
        }
    }
}
