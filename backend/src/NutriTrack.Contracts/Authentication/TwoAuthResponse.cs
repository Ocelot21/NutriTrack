using System;
using System.Collections.Generic;
using System.Text;

namespace NutriTrack.Contracts.Authentication;

public sealed record SendTwoFactorCodeResponse(
string Delivery,         
string? MaskedDestination,     
string? Code    
);
