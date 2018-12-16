using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PortalTeme.Auth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.Auth.Pages {
    public class ConsentModel : PageModel {
        private readonly IIdentityServerInteractionService interactionService;
        private readonly IClientStore clientStore;
        private readonly IResourceStore resourceStore;
        private readonly ILogger<ConsentModel> logger;

        // Output
        public string ClientName { get; set; }
        public string ClientUrl { get; set; }
        public string ClientLogoUrl { get; set; }
        public bool AllowRememberConsent { get; set; }
        public IEnumerable<ScopeViewModel> IdentityScopes { get; set; }
        public IEnumerable<ScopeViewModel> ResourceScopes { get; set; }

        // Input
        public bool RememberConsent { get; set; }
        public string ReturnUrl { get; set; }


        public ConsentModel(IIdentityServerInteractionService interactionService,
            IClientStore clientStore,
            IResourceStore resourceStore,
            ILogger<ConsentModel> logger) {
            this.interactionService = interactionService;
            this.clientStore = clientStore;
            this.resourceStore = resourceStore;
            this.logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl) {
            if (!await BuildViewModelAsync(returnUrl))
                return RedirectToPage("/Error", new { Mesage = "Failed to render the Consent page." });
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(ConsentInputModel input) {
            var result = await ProcessConsent(input, false);
            return HandleResult(result);
        }


        public async Task<IActionResult> OnPostCancelAsync(ConsentInputModel input) {
            var result = await ProcessConsent(input, true);
            return HandleResult(result);
        }

        private IActionResult HandleResult(ProcessConsentResult result) {
            if (result.IsRedirect) {
                return Redirect(result.RedirectUri);
            }

            if (result.HasValidationError) {
                ModelState.AddModelError("", result.ValidationError);
            }

            if (result.ShowView) {
                return Page();
            }

            return RedirectToPage("/Error", new { Message = "Could not process your consent." });
        }

        private async Task<ProcessConsentResult> ProcessConsent(ConsentInputModel model, bool canceled) {
            var result = new ProcessConsentResult();

            ConsentResponse grantedConsent = null;

            // user canceled the consent page - send back the standard 'access_denied' response
            if (canceled) {
                grantedConsent = ConsentResponse.Denied;
            }
            // user clicked 'yes' - validate the data
            else if (model != null) {
                // if the user consented to some scope, build the response model
                if (model.ScopesConsented != null && model.ScopesConsented.Any()) {
                    var scopes = model.ScopesConsented;
                    if (ConsentOptions.EnableOfflineAccess == false) {
                        scopes = scopes.Where(x => x != IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess);
                    }

                    grantedConsent = new ConsentResponse {
                        RememberConsent = model.RememberConsent,
                        ScopesConsented = scopes.ToArray()
                    };
                } else {
                    result.ValidationError = ConsentOptions.MustChooseOneErrorMessage;
                }
            } else {
                result.ValidationError = ConsentOptions.InvalidSelectionErrorMessage;
            }

            if (grantedConsent != null) {
                // validate return url is still valid
                var request = await interactionService.GetAuthorizationContextAsync(model.ReturnUrl);
                if (request == null) return result;

                // communicate outcome of consent back to identityserver
                await interactionService.GrantConsentAsync(request, grantedConsent);

                // indicate that's it ok to redirect back to authorization endpoint
                result.RedirectUri = model.ReturnUrl;
            } else {
                // we need to redisplay the consent UI
                result.ShowView = await BuildViewModelAsync(model.ReturnUrl, model);
            }

            return result;
        }

        private async Task<bool> BuildViewModelAsync(string returnUrl, ConsentInputModel model = null) {
            var request = await interactionService.GetAuthorizationContextAsync(returnUrl);
            if (request != null) {
                var client = await clientStore.FindEnabledClientByIdAsync(request.ClientId);
                if (client != null) {
                    var resources = await resourceStore.FindEnabledResourcesByScopeAsync(request.ScopesRequested);
                    if (resources != null && (resources.IdentityResources.Any() || resources.ApiResources.Any())) {
                        CreateConsentViewModel(model, returnUrl, request, client, resources);
                        return true;
                    } else {
                        logger.LogError("No scopes matching: {0}", request.ScopesRequested.Aggregate((x, y) => x + ", " + y));
                    }
                } else {
                    logger.LogError("Invalid client id: {0}", request.ClientId);
                }
            } else {
                logger.LogError("No consent request matching request: {0}", returnUrl);
            }

            return false;
        }

        private void CreateConsentViewModel(ConsentInputModel model,
            string returnUrl,
            AuthorizationRequest request,
            Client client, Resources resources) {

            RememberConsent = model?.RememberConsent ?? true;
            var ScopesConsented = model?.ScopesConsented ?? Enumerable.Empty<string>();

            ReturnUrl = returnUrl;

            ClientName = client.ClientName ?? client.ClientId;
            ClientUrl = client.ClientUri;
            ClientLogoUrl = client.LogoUri;
            AllowRememberConsent = client.AllowRememberConsent;

            IdentityScopes = resources.IdentityResources
                .Select(x => CreateScopeViewModel(x, ScopesConsented.Contains(x.Name) || model == null))
                .ToArray();
            ResourceScopes = resources.ApiResources
                .SelectMany(x => x.Scopes)
                .Select(x => CreateScopeViewModel(x, ScopesConsented.Contains(x.Name) || model == null))
                .ToArray();
            if (ConsentOptions.EnableOfflineAccess && resources.OfflineAccess) {
                ResourceScopes = ResourceScopes.Union(new ScopeViewModel[] {
                    GetOfflineAccessScope(ScopesConsented.Contains(IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess) || model == null)
                });
            }
        }

        private ScopeViewModel CreateScopeViewModel(IdentityResource identity, bool check) {
            return new ScopeViewModel {
                Name = identity.Name,
                DisplayName = identity.DisplayName,
                Description = identity.Description,
                Emphasize = identity.Emphasize,
                Required = identity.Required,
                Checked = check || identity.Required
            };
        }

        private ScopeViewModel CreateScopeViewModel(Scope scope, bool check) {
            return new ScopeViewModel {
                Name = scope.Name,
                DisplayName = scope.DisplayName,
                Description = scope.Description,
                Emphasize = scope.Emphasize,
                Required = scope.Required,
                Checked = check || scope.Required
            };
        }

        private ScopeViewModel GetOfflineAccessScope(bool check) {
            return new ScopeViewModel {
                Name = IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess,
                DisplayName = ConsentOptions.OfflineAccessDisplayName,
                Description = ConsentOptions.OfflineAccessDescription,
                Emphasize = true,
                Checked = check
            };
        }

    }

    public class ProcessConsentResult {
        public bool IsRedirect => RedirectUri != null;
        public string RedirectUri { get; set; }

        public bool ShowView { get; set; }

        public bool HasValidationError => ValidationError != null;
        public string ValidationError { get; set; }
    }

    public class ConsentInputModel {
        public IEnumerable<string> ScopesConsented { get; set; }
        public bool RememberConsent { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class ConsentViewModel : ConsentInputModel {
        public string ClientName { get; set; }
        public string ClientUrl { get; set; }
        public string ClientLogoUrl { get; set; }
        public bool AllowRememberConsent { get; set; }

        public IEnumerable<ScopeViewModel> IdentityScopes { get; set; }
        public IEnumerable<ScopeViewModel> ResourceScopes { get; set; }
    }

    public class ScopeViewModel {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Emphasize { get; set; }
        public bool Required { get; set; }
        public bool Checked { get; set; }
    }
}