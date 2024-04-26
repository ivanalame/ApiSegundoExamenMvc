using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace ApiSegundoExamenMvc.Helpers
{
    public class HelperActionServicesOAuth
    {

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string SecretKey { get; set; }



        public HelperActionServicesOAuth(IConfiguration configuration)

        {
             var keyVaultUri = configuration.GetValue<string>("ApiOauth:KeyVaultUri");
            var secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());

            //this.Issuer =

            //    configuration.GetValue<string>("ApiOAuth:Issuer");

            //this.Audience =

            //    configuration.GetValue<string>("ApiOAuth:Audience");

            //this.SecretKey =

            //    configuration.GetValue<string>("ApiOAuth:SecretKey");

            Issuer = getsecretvalue(secretClient, "secretoIssuer");
            Audience = getsecretvalue(secretClient, "secretoAudience");
            SecretKey = getsecretvalue(secretClient, "secretoSecretKey");

        }

        private string getsecretvalue(SecretClient secretclient, string secretname)
        {
            try
            {
                KeyVaultSecret secret = secretclient.GetSecret(secretname);
                return secret.Value;
            }
            catch (Exception ex)
            {
                // maneja la excepción según sea necesario
                throw new Exception($"no se pudo obtener el secreto '{secretname}' del key vault.", ex);
            }
        }


        //NECESITAMOS UN METODO PARA GENERAR EL TOKEN  

        //QUE SE BASA EN EL SECRET KEY 

        public SymmetricSecurityKey GetKeyToken()

        {

            //CONVERTIMOS EL SECRET KEY A BYTES[] 

            byte[] data =

                Encoding.UTF8.GetBytes(this.SecretKey);

            //DEVOLVEMOS LA KEY GENERADA MEDIANTE LOS bytes[] 

            return new SymmetricSecurityKey(data);

        }




        public Action<JwtBearerOptions> GetJwtBearerOptions()

        {

            Action<JwtBearerOptions> options =

                new Action<JwtBearerOptions>(options =>

                {

                    //INDICAMOS QUE DESEAMOS VALIDAR DE  

                    //NUESTRO TOKEN, ISSUER, AUDIENCE 

                    //, TIME 

                    options.TokenValidationParameters =

                    new TokenValidationParameters

                    {

                        ValidateIssuer = true,

                        ValidateAudience = true,

                        ValidateLifetime = true,

                        ValidateIssuerSigningKey = true,

                        ValidIssuer = this.Issuer,

                        ValidAudience = this.Audience,

                        IssuerSigningKey = this.GetKeyToken()

                    };

                });

            return options;

        }



        //METODO PARA INDICAR EL ESQUEMA DE LA VALIDACION 

        public Action<AuthenticationOptions>

            GetAuthenticateSchema()

        {

            Action<AuthenticationOptions> options =

                new Action<AuthenticationOptions>(options =>

                {

                    options.DefaultScheme =

                    JwtBearerDefaults.AuthenticationScheme;

                    options.DefaultAuthenticateScheme =

                    JwtBearerDefaults.AuthenticationScheme;

                    options.DefaultChallengeScheme =

                    JwtBearerDefaults.AuthenticationScheme;

                });

            return options;

        }

    }
}