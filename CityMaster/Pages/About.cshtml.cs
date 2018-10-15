﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CityMaster.Pages
{
	public class AboutModel : PageModelBase
	{
		public string Message { get; set; }

		public void OnGet()
		{
			Message = "Your application description page.";
		}
	}
}
