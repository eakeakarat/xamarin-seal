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
        CKKSEncoder encoder;
        Evaluator evaluator;
        Encryptor encryptor;
        Decryptor decryptor;
        BatchEncoder batchEncoder;

        public MainPage()
        {
            InitializeComponent();

            //CKKS
            //using EncryptionParameters parms = new EncryptionParameters(SchemeType.CKKS);
            //ulong polyModulusDegree = 8192;

            //BFV
            using EncryptionParameters parms = new EncryptionParameters(SchemeType.BFV);
            ulong polyModulusDegree = 4096;

            parms.PolyModulusDegree = polyModulusDegree;

            //CKKS
            //parms.CoeffModulus = CoeffModulus.Create(polyModulusDegree, new int[] { 60, 40, 40, 60 });
            //parms.PlainModulus = new Modulus(1024);

            //BFV
            parms.CoeffModulus = CoeffModulus.BFVDefault(polyModulusDegree);
            parms.PlainModulus = PlainModulus.Batching(polyModulusDegree, 20);

            using SEALContext context = new SEALContext(parms);
            System.Console.WriteLine(context.KeyContextData.ParmsId);
            System.Console.WriteLine("OKOK");

            using KeyGenerator keygen = new KeyGenerator(context);
            using SecretKey secretKey = keygen.SecretKey;
            keygen.CreatePublicKey(out PublicKey publicKey);

            evaluator = new Evaluator(context);
            encryptor = new Encryptor(context, publicKey);
            decryptor = new Decryptor(context, secretKey);

            //CKKS
            //encoder = new CKKSEncoder(context);

            //BFV
            batchEncoder = new BatchEncoder(context);

            using Plaintext plainA = new Plaintext(),
                           plainB = new Plaintext(),
                           plainC = new Plaintext();
            Ciphertext cipherA = new Ciphertext(),
                cipherB = new Ciphertext(),
                cipherC = new Ciphertext();

            //CKKS
            //encoder.Encode(2, scale, plainA);
            //encoder.Encode(3, scale, plainB);
            //encryptor.Encrypt(plainA, cipherA);
            //encryptor.Encrypt(plainB, cipherB);

            //BFV
            ulong[] a = { 1,2,3 }, b = { 2,3,4 };
            batchEncoder.Encode(a, plainA);
            batchEncoder.Encode(b, plainB);
            encryptor.Encrypt(plainA, cipherA);
            encryptor.Encrypt(plainB, cipherB);

            evaluator.Multiply(cipherA, cipherB, cipherC);
            decryptor.Decrypt(cipherC, plainC);

            String resultSTR = "";

            //CKKS
            //ICollection<double> l = new List<double>();
            //encoder.Decode(plainC, l);
            //foreach (double x in l)
            //{
            //    string tmp = x.ToString();
            //    resultSTR += tmp + '\n';

            //}

            //BFV
            List<ulong> l = new List<ulong>();
            batchEncoder.Decode(plainC, l);
            foreach (ulong x in l)
            {
                string tmp = x.ToString();
                resultSTR += tmp + '\n';
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
