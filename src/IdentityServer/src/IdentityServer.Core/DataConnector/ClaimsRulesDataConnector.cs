using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using IdOps.IdentityServer.Model;
using IdOps.IdentityServer.Services;

namespace IdOps.IdentityServer.DataConnector
{
    public class ClaimsRulesDataConnector : IUserDataConnector
    {
        private readonly IUserClaimsRulesService _userClaimsRulesService;
        private readonly IdOpsOptions _idOpsOptions;

        public ClaimsRulesDataConnector(
            IUserClaimsRulesService userClaimsRulesService,
            IdOpsOptions idOpsOptions)
        {
            _userClaimsRulesService = userClaimsRulesService;
            _idOpsOptions = idOpsOptions;
        }

        public string Name => "USER_CLAIMS_RULES";

        public async Task<UserDataConnectorResult> GetClaimsAsync(
            UserDataConnectorCallerContext context,
            DataConnectorOptions options,
            IEnumerable<Claim> input,
            CancellationToken cancellationToken)
        {
            var result = new UserDataConnectorResult { Success = true };
            var newClaims = new List<Claim>();

            try
            {
                IEnumerable<string>? inputClaimTypes = input.Select(x => x.Type).Distinct();
                result.CacheKey = (context.Subject + context.Client!.ClientId).Sha256();

                IReadOnlyList<UserClaimRule>? rules = await _userClaimsRulesService
                    .GetRulesByClientAsync(
                        context.Client!,
                        inputClaimTypes,
                        cancellationToken);

                foreach (UserClaimRule? rule in rules)
                {
                    IEnumerable<Claim> ruleClaims = GetRuleClaims(rule, input);
                    newClaims.AddRange(ruleClaims);
                }

                result.Claims = newClaims.Distinct();
                result.Executed = true;
            }
            catch (Exception ex)
            {
                result.Error = ex;
                result.Success = false;
            }

            return result;
        }

        private IEnumerable<Claim> GetRuleClaims(
            UserClaimRule rule,
            IEnumerable<Claim> input)
        {
            bool isMatch = IsMatch(rule, input);
            if (isMatch)
            {
                return rule.Claims.Select(x => new Claim(x.Type, x.Value));
            }

            return Array.Empty<Claim>();
        }

        private bool IsMatch(UserClaimRule rule, IEnumerable<Claim> input)
        {
            foreach (ClaimRuleMatch? ruleMatch in rule.Rules)
            {
                foreach (Claim claim in input.Where( x => x.Type == ruleMatch.ClaimType))
                {
                    var isValueMatch = IsValueMatch(ruleMatch, claim.Value);

                    if (isValueMatch)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsValueMatch(ClaimRuleMatch ruleMatch, string value)
        {
            switch (ruleMatch.MatchMode)
            {
                case ClaimRuleMatchMode.Equals:
                    return ruleMatch.Value.Equals(
                        value,
                        StringComparison.InvariantCultureIgnoreCase);
                case ClaimRuleMatchMode.StartWith:
                    return ruleMatch.Value.StartsWith(
                        value,
                        StringComparison.InvariantCultureIgnoreCase);
                case ClaimRuleMatchMode.Contains:
                    return ruleMatch.Value.Contains(
                        value,
                        StringComparison.InvariantCultureIgnoreCase);
                case ClaimRuleMatchMode.EndsWith:
                    return ruleMatch.Value.EndsWith(
                        value,
                        StringComparison.InvariantCultureIgnoreCase);
                case ClaimRuleMatchMode.Regex:
                    return Regex.IsMatch(value, ruleMatch.Value);
                case ClaimRuleMatchMode.OneOf:
                    var values = value.Split(",");
                    return values.Contains(value);
                default:
                    return false;
            }
        }
    }
}
