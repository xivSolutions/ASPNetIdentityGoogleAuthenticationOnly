﻿using ASPNetIdentityGoogleAuthenticationOnly.IdentityConfig;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace ASPNetIdentityGoogleAuthenticationOnly.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        static ApplicationDbContext()
        {
            // Set the database intializer which is run once during application start
            // This seeds the database with admin user credentials and admin role
            Database.SetInitializer(new ApplicationDbInitializer());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}