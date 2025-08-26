using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AADBOT_KarloHruskovec.DTOs;
using Xunit;

namespace AADBOT.Tests
{
	public class DtoAttributesTests
	{
		bool Has<TAttr>(Type t, string prop) where TAttr : Attribute
		{
			var pi = t.GetProperty(prop);
			Assert.NotNull(pi); // fail clearly if the property is missing
			return pi!.GetCustomAttributes(typeof(TAttr), true).Any();
		}

		[Fact]
		public void Login_Email_HasRequiredAndEmailAddress()
		{
			var t = typeof(LoginRequest);
			Assert.True(Has<RequiredAttribute>(t, "Email"));
			Assert.True(Has<EmailAddressAttribute>(t, "Email"));
		}

		[Fact]
		public void Login_Password_HasRequired()
		{
			var t = typeof(LoginRequest);
			Assert.True(Has<RequiredAttribute>(t, "Password"));
		}

		[Fact]
		public void Register_Email_HasRequiredAndEmailAddress()
		{
			var t = typeof(RegisterRequest);
			Assert.True(Has<RequiredAttribute>(t, "Email"));
			Assert.True(Has<EmailAddressAttribute>(t, "Email"));
		}

		[Fact]
		public void Register_Password_HasMinLength()
		{
			var t = typeof(RegisterRequest);
			Assert.True(Has<MinLengthAttribute>(t, "Password"));
		}

		[Fact]
		public void Register_Package_HasRequired()
		{
			var t = typeof(RegisterRequest);
			Assert.True(Has<RequiredAttribute>(t, "Package"));
		}

		[Fact]
		public void RefreshToken_Token_HasRequired()
		{
			var t = typeof(RefreshTokenRequest);
			Assert.True(Has<RequiredAttribute>(t, "RefreshToken"));
		}
	}
}
