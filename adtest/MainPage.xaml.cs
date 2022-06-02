using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Microsoft.Research.SEAL;
using System.IO;

namespace adtest
{
    public partial class MainPage : ContentPage
    {
        double scale = Math.Pow(2.0, 40);
        EncryptionParameters parms;
        BatchEncoder batchEncoder;
        CKKSEncoder encoder;
        SEALContext context;
        KeyGenerator keygen;
        PublicKey publicKey;
        SecretKey secretKey;
        Evaluator evaluator;
        Encryptor encryptor;
        Decryptor decryptor;

        public MainPage()
        {
            InitializeComponent();
            // BFV | CKKS 
            bool BFV = true;

            ulong polyModulusDegree;

            if (BFV)
            {
                //BFV
                parms = new EncryptionParameters(SchemeType.BFV);
                polyModulusDegree = 4096;
            }
            else
            {
                //CKKS
                parms = new EncryptionParameters(SchemeType.CKKS);
                polyModulusDegree = 8192;
            }

            parms.PolyModulusDegree = polyModulusDegree;

            if (BFV)
            {
                //BFV
                parms.CoeffModulus = CoeffModulus.BFVDefault(polyModulusDegree);
                 parms.PlainModulus = PlainModulus.Batching(polyModulusDegree, 20);
            }
            else
            {
                //CKKS
                parms.CoeffModulus = CoeffModulus.Create(polyModulusDegree, new int[] { 60, 40, 40, 60 });
                 parms.PlainModulus = new Modulus(1024);
            }

            context = new SEALContext(parms);
            keygen = new KeyGenerator(context);
            secretKey = keygen.SecretKey;
            keygen.CreatePublicKey(out publicKey);

            evaluator = new Evaluator(context);
            encryptor = new Encryptor(context, publicKey);
            decryptor = new Decryptor(context, secretKey);

            if (BFV)
            {
                //BFV
                batchEncoder = new BatchEncoder(context);
            }
            else
            {
                //CKKS
                encoder = new CKKSEncoder(context);
            }

            Plaintext plainA = new Plaintext(),
                           plainB = new Plaintext(),
                           plainC = new Plaintext();
            Ciphertext cipherA = new Ciphertext(),
                cipherB = new Ciphertext(),
                cipherC = new Ciphertext();



            if (BFV)
            {
                //BFV
                ulong[] a = { 1, 2, 3 }, b = { 2, 3, 4 }, c = { 2, 2, 2 };
                batchEncoder.Encode(a, plainA);
                batchEncoder.Encode(b, plainB);
                batchEncoder.Encode(c, plainC);
                encryptor.Encrypt(plainA, cipherA);
                encryptor.Encrypt(plainB, cipherB);
                encryptor.Encrypt(plainC, cipherC);
            }
            else
            {
                //CKKS
                encoder.Encode(2, scale, plainA);
                encoder.Encode(3, scale, plainB);
                encoder.Encode(4, scale, plainC);
                encryptor.Encrypt(plainA, cipherA);
                encryptor.Encrypt(plainB, cipherB);
                encryptor.Encrypt(plainC, cipherC);
            }

            evaluator.Add(cipherA, cipherB, cipherA); // A = A + B ==> {3,5,7} || 5
            evaluator.Multiply(cipherA, cipherC, cipherC); // C = A * C ==> {6,10,14} || 20
            decryptor.Decrypt(cipherC, plainC);

            String resultSTR = "";

            if (BFV)
            {
                //BFV
                List<ulong> l = new List<ulong>();
                batchEncoder.Decode(plainC, l);
                foreach (ulong x in l)
                {
                    string tmp = x.ToString();
                    resultSTR += tmp + '\n';
                }
            }
            else
            {
                //CKKS
                ICollection<double> l = new List<double>();
                encoder.Decode(plainC, l);
                foreach (double x in l)
                {
                    string tmp = x.ToString();
                    resultSTR += tmp + '\n';

                }

            }

            result.Text = resultSTR;

        }

        private void submit_Clicked(object sender, EventArgs e)
        {
            var a = entry1.Text;
            var b = entry2.Text;
            int j = int.Parse(a);
            int k = int.Parse(b);
            int l = j + k;
            result.Text = l.ToString();

            //Plaintext A = new Plaintext(), B = new Plaintext();
            //Ciphertext cA = new Ciphertext(), cB = new Ciphertext();

            //encoder.Encode(j, scale, A);
            //encoder.Encode(k, scale, B);

            //Plaintext xPlain = new Plaintext();
            //Ciphertext xCipher = new Ciphertext();
            //encoder.Encode(input, scale, xPlain);

            //encryptor.Encrypt(A, cA);
            //encryptor.Encrypt(B, cB);

            //evaluator.Add(cA, cB, xCipher);

            //decryptor.Decrypt(xCipher, xPlain);
            //ICollection<double> r = new List<double>();
            //encoder.Decode(xPlain, r);

            //result.Text = r.ToString();


        }

    }
}
