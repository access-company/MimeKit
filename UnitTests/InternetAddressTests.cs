//
// InternetAddressTests.cs
//
// Author: Jeffrey Stedfast <jestedfa@microsoft.com>
//
// Copyright (c) 2013-2017 Xamarin Inc. (www.xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using System.Text;

using NUnit.Framework;

using MimeKit;

namespace UnitTests {
	[TestFixture]
	public class InternetAddressTests
	{
		static void AssertParseFailure (string text, bool result, int tokenIndex, int errorIndex)
		{
			var buffer = text.Length > 0 ? Encoding.ASCII.GetBytes (text) : new byte[1];
			InternetAddress address;

			Assert.AreEqual (result, InternetAddress.TryParse (text, out address), "InternetAddress.TryParse(string)");
			Assert.AreEqual (result, InternetAddress.TryParse (buffer, out address), "InternetAddress.TryParse(byte[])");
			Assert.AreEqual (result, InternetAddress.TryParse (buffer, 0, out address), "InternetAddress.TryParse(byte[], int)");
			Assert.AreEqual (result, InternetAddress.TryParse (buffer, 0, buffer.Length, out address), "InternetAddress.TryParse(byte[], int, int)");

			try {
				InternetAddress.Parse (text);
				Assert.Fail ("InternetAddress.Parse(string) should fail.");
			} catch (ParseException ex) {
				Assert.AreEqual (tokenIndex, ex.TokenIndex, "ParseException did not have the correct token index.");
				Assert.AreEqual (errorIndex, ex.ErrorIndex, "ParseException did not have the error index.");
			} catch {
				Assert.Fail ("InternetAddress.Parse(string) should throw ParseException.");
			}

			try {
				InternetAddress.Parse (buffer);
				Assert.Fail ("InternetAddress.Parse(byte[]) should fail.");
			} catch (ParseException ex) {
				Assert.AreEqual (tokenIndex, ex.TokenIndex, "ParseException did not have the correct token index.");
				Assert.AreEqual (errorIndex, ex.ErrorIndex, "ParseException did not have the error index.");
			} catch {
				Assert.Fail ("InternetAddress.Parse(new byte[]) should throw ParseException.");
			}

			try {
				InternetAddress.Parse (buffer, 0);
				Assert.Fail ("InternetAddress.Parse(byte[], int) should fail.");
			} catch (ParseException ex) {
				Assert.AreEqual (tokenIndex, ex.TokenIndex, "ParseException did not have the correct token index.");
				Assert.AreEqual (errorIndex, ex.ErrorIndex, "ParseException did not have the error index.");
			} catch {
				Assert.Fail ("InternetAddress.Parse(new byte[], int) should throw ParseException.");
			}

			try {
				InternetAddress.Parse (buffer, 0, buffer.Length);
				Assert.Fail ("InternetAddress.Parse(byte[], int, int) should fail.");
			} catch (ParseException ex) {
				Assert.AreEqual (tokenIndex, ex.TokenIndex, "ParseException did not have the correct token index.");
				Assert.AreEqual (errorIndex, ex.ErrorIndex, "ParseException did not have the error index.");
			} catch {
				Assert.Fail ("InternetAddress.Parse(new byte[], int, int) should throw ParseException.");
			}
		}

		static void AssertParse (string text)
		{
			var buffer = Encoding.ASCII.GetBytes (text);
			InternetAddress address;

			try {
				Assert.IsTrue (InternetAddress.TryParse (text, out address), "InternetAddress.TryParse(string) should succeed.");
			} catch (Exception ex) {
				Assert.Fail ("InternetAddress.TryParse(string) should not throw an exception: {0}", ex);
			}

			try {
				Assert.IsTrue (InternetAddress.TryParse (buffer, out address), "InternetAddress.TryParse(byte[]) should succeed.");
			} catch (Exception ex) {
				Assert.Fail ("InternetAddress.TryParse(byte[]) should not throw an exception: {0}", ex);
			}

			try {
				Assert.IsTrue (InternetAddress.TryParse (buffer, 0, out address), "InternetAddress.TryParse(byte[], int) should succeed.");
			} catch (Exception ex) {
				Assert.Fail ("InternetAddress.TryParse(byte[], int) should not throw an exception: {0}", ex);
			}

			try {
				Assert.IsTrue (InternetAddress.TryParse (buffer, 0, buffer.Length, out address), "InternetAddress.TryParse(byte[], int, int) should succeed.");
			} catch (Exception ex) {
				Assert.Fail ("InternetAddress.TryParse(byte[], int, int) should not throw an exception: {0}", ex);
			}

			try {
				address = InternetAddress.Parse (text);
			} catch (Exception ex) {
				Assert.Fail ("InternetAddress.Parse(string) should not throw an exception: {0}", ex);
			}

			try {
				address = InternetAddress.Parse (buffer);
			} catch (Exception ex) {
				Assert.Fail ("InternetAddress.Parse(string) should not throw an exception: {0}", ex);
			}

			try {
				address = InternetAddress.Parse (buffer, 0);
			} catch (Exception ex) {
				Assert.Fail ("InternetAddress.Parse(string) should not throw an exception: {0}", ex);
			}

			try {
				address = InternetAddress.Parse (buffer, 0, buffer.Length);
			} catch (Exception ex) {
				Assert.Fail ("InternetAddress.Parse(string) should not throw an exception: {0}", ex);
			}
		}

		[Test]
		public void TestParseEmpty ()
		{
			AssertParseFailure (string.Empty, false, 0, 0);
		}

		[Test]
		public void TestParseWhiteSpace ()
		{
			const string text = " \t\r\n";
			int tokenIndex = text.Length;
			int errorIndex = text.Length;

			AssertParseFailure (text, false, tokenIndex, errorIndex);
		}

		[Test]
		public void TestParseNameLessThan ()
		{
			const string text = "Name <";
			const int tokenIndex = 0;
			int errorIndex = text.Length;

			AssertParseFailure (text, false, tokenIndex, errorIndex);
		}

		[Test]
		public void TestParseMailboxWithEmptyDomain ()
		{
			const string text = "jeff@";
			const int tokenIndex = 0;
			int errorIndex = text.Length;

			AssertParseFailure (text, false, tokenIndex, errorIndex);
		}

		[Test]
		public void TestParseMailboxWithIncompleteLocalPart ()
		{
			const string text = "jeff.";
			const int tokenIndex = 0;
			int errorIndex = text.Length;

			AssertParseFailure (text, false, tokenIndex, errorIndex);
		}

		[Test]
		public void TestParseIncompleteQuotedString ()
		{
			const string text = "\"This quoted string never ends... oh no!";

			AssertParse (text);
		}

		[Test]
		public void TestParseMailboxWithIncompleteCommentAfterName ()
		{
			const string text = "Name (incomplete comment";
			int tokenIndex = text.IndexOf ('(');
			int errorIndex = text.Length;

			AssertParseFailure (text, false, tokenIndex, errorIndex);
		}

		[Test]
		public void TestParseMailboxWithIncompleteCommentAfterAddrspec ()
		{
			const string text = "jeff@xamarin.com (incomplete comment";
			int tokenIndex = text.IndexOf ('(');
			int errorIndex = text.Length;

			AssertParseFailure (text, false, tokenIndex, errorIndex);
		}

		[Test]
		public void TestParseMailboxWithIncompleteCommentAfterAddress ()
		{
			const string text = "<jeff@xamarin.com> (incomplete comment";
			int tokenIndex = text.IndexOf ('(');
			int errorIndex = text.Length;

			AssertParseFailure (text, false, tokenIndex, errorIndex);
		}

		[Test]
		public void TestParseIncompleteAddrspec ()
		{
			const string text = "jeff@ (comment)";
			const int tokenIndex = 0;
			int errorIndex = text.Length;

			AssertParseFailure (text, false, tokenIndex, errorIndex);
		}

		[Test]
		public void TestParseAddrspecNoAtDomain ()
		{
			const string text = "jeff";

			AssertParse (text);
		}

		[Test]
		public void TestParseAddrspec ()
		{
			const string text = "jeff@xamarin.com";

			AssertParse (text);
		}

		[Test]
		public void TestParseMailbox ()
		{
			const string text = "Jeffrey Stedfast <jestedfa@microsoft.com>";

			AssertParse (text);
		}

		[Test]
		public void TestParseMailboxWithUnquotedCommaAndDotInName ()
		{
			const string text = "Warren Worthington, Jr. <warren@worthington.com>";

			AssertParse (text);
		}

		[Test]
		public void TestParseMailboxWithUnquotedCommaInName ()
		{
			const string text = "Worthington, Warren <warren@worthington.com>";
			InternetAddress addr;

			AssertParse (text);

			// default options should parse this as a single mailbox address
			addr = InternetAddress.Parse (text);
			Assert.AreEqual ("Worthington, Warren", addr.Name);
		}

		[Test]
		public void TestParseMailboxWithOpenAngleSpace ()
		{
			const string text = "Jeffrey Stedfast < jeff@xamarin.com>";

			AssertParse (text);
		}

		[Test]
		public void TestParseMailboxWithCloseAngleSpace ()
		{
			const string text = "Jeffrey Stedfast <jeff@xamarin.com >";

			AssertParse (text);
		}

		[Test]
		public void TestParseMailboxWithIncompleteRoute ()
		{
			const string text = "Skye <@";
			const int tokenIndex = 0;
			int errorIndex = text.Length;

			AssertParseFailure (text, false, tokenIndex, errorIndex);
		}

		[Test]
		public void TestParseMailboxWithoutColonAfterRoute ()
		{
			const string text = "Skye <@hackers.com,@shield.gov";
			const int tokenIndex = 0;
			int errorIndex = text.Length;

			AssertParseFailure (text, false, tokenIndex, errorIndex);
		}

		[Test]
		public void TestParseMultipleMailboxes ()
		{
			const string text = "Skye <skye@shield.gov>, Leo Fitz <fitz@shield.gov>, Melinda May <may@shield.gov>";
			int tokenIndex = text.IndexOf (',');
			int errorIndex = tokenIndex;

			AssertParseFailure (text, false, tokenIndex, errorIndex);
		}

		[Test]
		public void TestParseGroup ()
		{
			const string text = "Agents of Shield: Skye <skye@shield.gov>, Leo Fitz <fitz@shield.gov>, Melinda May <may@shield.gov>;";

			AssertParse (text);
		}

		[Test]
		public void TestParseIncompleteGroup ()
		{
			const string text = "Agents of Shield: Skye <skye@shield.gov>, Leo Fitz <fitz@shield.gov>, May <may@shield.gov>";

			AssertParse (text);
		}

		[Test]
		public void TestParseGroupNameColon ()
		{
			const string text = "Agents of Shield:";
			int tokenIndex = text.Length;
			int errorIndex = text.Length;

			// Note: the TryParse() methods are a little more forgiving than Parse().
			AssertParseFailure (text, true, tokenIndex, errorIndex);
		}

		[Test]
		public void TestParseGroupAndMailbox ()
		{
			const string text = "Agents of Shield: Skye <skye@shield.gov>, Leo Fitz <fitz@shield.gov>, May <may@shield.gov>;, Fury <fury@shield.gov>";
			int tokenIndex = text.IndexOf (';') + 1;
			int errorIndex = tokenIndex;

			AssertParseFailure (text, false, tokenIndex, errorIndex);
		}

		#region Rfc7103

		// TODO: test both Strict and Loose RfcCompliance modes

		[Test]
		public void TestParseMailboxWithExcessiveAngleBrackets ()
		{
			const string text = "<<<user2@example.org>>>";

			AssertParse (text);
		}

		[Test]
		public void TestParseMailboxWithMissingGreaterThan ()
		{
			const string text = "<another@example.net";

			AssertParse (text);
		}

		[Test]
		public void TestParseMailboxWithMissingLessThan ()
		{
			const string text = "second@example.org>";

			AssertParse (text);
		}

		[Test]
		public void TestParseMailboxWithUnbalancedQuotes ()
		{
			const string text = "\"Joe <joe@example.com>";

			AssertParse (text);
		}

		[Test]
		public void TestParseMailboxWithAddrspecAsUnquotedName ()
		{
			const string text = "user@example.com <user@example.com>";

			AssertParse (text);
		}

		#endregion

		#region TestLegacyEmailAddress
		TestCaseData[] LegacyAddressNotCompliantWithRFC ()
		{
			var addressList = new TestCaseData[]
			{
				// RFCCompliance
				new TestCaseData ("aaaa@example.com"),
				new TestCaseData ("aa.aa@example.com"),
				new TestCaseData ("0123456789@example.com"),
				new TestCaseData ("abcdefghijklmnopqrstuvwxyz@example.com"),
				new TestCaseData ("ABCDEFGHIJKLMNOPQRSTUVWXYZ@example.com"),
				new TestCaseData ("a.a-a_a@ex-amp_le.com"),
				new TestCaseData ("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa@example.com"),
				new TestCaseData ("アドレス <aaaa@example.com>"),
				new TestCaseData ("アドレス<aaaa@example.com>"),
				new TestCaseData ("\"アドレス\" <aaaa@example.com>"),
				new TestCaseData ("\"アドレス\"   <aaaa@example.com>"),
				new TestCaseData ("\"アドレス\"<aaaa@example.com>"),
				new TestCaseData ("aaaa@example.com (アドレス)"),
				new TestCaseData ("aaaa@example.com   (アドレス)"),
				new TestCaseData ("aaaa@example.com(アドレス)"),
				new TestCaseData ("0123456789 <aaaa@example.com>"),
				new TestCaseData ("abcdefghijklmnopqrstuvwxyz <aaaa@example.com>"),
				new TestCaseData ("ABCDEFGHIJKLMNOPQRSTUVWXYZ <aaaa@example.com>"),
				new TestCaseData ("a.a@a-a_a <aaaa@example.com>"),
				new TestCaseData ("bbb@example.com <aaaa@example.com>"),
				new TestCaseData ("%22%3E%3C%28%29%5C <aaaa@example.com>"),
				new TestCaseData ("\"%22%3E%3C%28%29%5C\" <aaaa@example.com>"),
				new TestCaseData ("aaaa@example.com (%22%3E%3C%28%29%5C)"),
				new TestCaseData ("aa!aa@example.com"),
				new TestCaseData ("aa#aa@example.com"),
				new TestCaseData ("aa$aa@example.com"),
				new TestCaseData ("aa%aa@example.com"),
				new TestCaseData ("aa&aa@example.com"),
				new TestCaseData ("aa'aa@example.com"),
				new TestCaseData ("aa*aa@example.com"),
				new TestCaseData ("aa+aa@example.com"),
				new TestCaseData ("aa/aa@example.com"),
				new TestCaseData ("aa=aa@example.com"),
				new TestCaseData ("aa?aa@example.com"),
				new TestCaseData ("aa^aa@example.com"),
				new TestCaseData ("aa`aa@example.com"),
				new TestCaseData ("aa{aa@example.com"),
				new TestCaseData ("aa|aa@example.com"),
				new TestCaseData ("aa}aa@example.com"),
				new TestCaseData ("aa~aa@example.com"),
				new TestCaseData ("aa%20aa@example.com"),
				new TestCaseData ("aa%21aa@example.com"),
				new TestCaseData ("aa%23aa@example.com"),
				new TestCaseData ("aa%24aa@example.com"),
				new TestCaseData ("aa%25aa@example.com"),
				new TestCaseData ("aa%26aa@example.com"),
				new TestCaseData ("aa%27aa@example.com"),
				new TestCaseData ("aa%2Aaa@example.com"),
				new TestCaseData ("aa%2Baa@example.com"),
				new TestCaseData ("aa%2Faa@example.com"),
				new TestCaseData ("aa%3Daa@example.com"),
				new TestCaseData ("aa%3Faa@example.com"),
				new TestCaseData ("aa%5Eaa@example.com"),
				new TestCaseData ("aa%60aa@example.com"),
				new TestCaseData ("aa%7Baa@example.com"),
				new TestCaseData ("aa%7Caa@example.com"),
				new TestCaseData ("aa%7Daa@example.com"),
				new TestCaseData ("aa%7Eaa@example.com"),
				new TestCaseData ("aa%00aa@example.com"),
				new TestCaseData ("aa%7Faa@example.com"),
				new TestCaseData ("アドレス <aa!aa@example.com>"),
				new TestCaseData ("アドレス <aa#aa@example.com>"),
				new TestCaseData ("アドレス <aa$aa@example.com>"),
				new TestCaseData ("アドレス <aa%aa@example.com>"),
				new TestCaseData ("アドレス <aa&aa@example.com>"),
				new TestCaseData ("アドレス <aa'aa@example.com>"),
				new TestCaseData ("アドレス <aa*aa@example.com>"),
				new TestCaseData ("アドレス <aa+aa@example.com>"),
				new TestCaseData ("アドレス <aa/aa@example.com>"),
				new TestCaseData ("アドレス <aa=aa@example.com>"),
				new TestCaseData ("アドレス <aa?aa@example.com>"),
				new TestCaseData ("アドレス <aa^aa@example.com>"),
				new TestCaseData ("アドレス <aa`aa@example.com>"),
				new TestCaseData ("アドレス <aa{aa@example.com>"),
				new TestCaseData ("アドレス <aa|aa@example.com>"),
				new TestCaseData ("アドレス <aa}aa@example.com>"),
				new TestCaseData ("アドレス <aa~aa@example.com>"),
				new TestCaseData ("アドレス <aa%21aa@example.com>"),
				new TestCaseData ("アドレス <aa%22aa@example.com>"),
				new TestCaseData ("アドレス <aa%23aa@example.com>"),
				new TestCaseData ("アドレス <aa%24aa@example.com>"),
				new TestCaseData ("アドレス <aa%25aa@example.com>"),
				new TestCaseData ("アドレス <aa%26aa@example.com>"),
				new TestCaseData ("アドレス <aa%27aa@example.com>"),
				new TestCaseData ("アドレス <aa%2Aaa@example.com>"),
				new TestCaseData ("アドレス <aa%2Baa@example.com>"),
				new TestCaseData ("アドレス <aa%2Faa@example.com>"),
				new TestCaseData ("アドレス <aa%3Daa@example.com>"),
				new TestCaseData ("アドレス <aa%3Faa@example.com>"),
				new TestCaseData ("アドレス <aa%5Eaa@example.com>"),
				new TestCaseData ("アドレス <aa%60aa@example.com>"),
				new TestCaseData ("アドレス <aa%7Baa@example.com>"),
				new TestCaseData ("アドレス <aa%7Caa@example.com>"),
				new TestCaseData ("アドレス <aa%7Daa@example.com>"),
				new TestCaseData ("アドレス <aa%7Eaa@example.com>"),
				new TestCaseData ("\"アド<>レス\" <aaaa@example.com>"),
				new TestCaseData ("\"アド()レス\" <aaaa@example.com>"),
				new TestCaseData ("ADDRESS012345"),
				new TestCaseData ("アドレス０１２３４５"),

				// Not compliant with RFC
				new TestCaseData ("aaaa.@example.com"),
				new TestCaseData ("aa..aa@example.com"),
				new TestCaseData (".aaaa@example.com"),
				new TestCaseData ("!\"#$%&'()=@amimap.access.co.jp"),
				new TestCaseData ("!\"#$%&'\"()=@amimap.access.co.jp"),
				new TestCaseData ("!\"#$%&'\"=\"@amimap.access.co.jp"),
				// More not compliant with RFC
				// Does not correspond now.
				//new TestCaseData ("アドレス <aa\"aa@example.com>"),
				//new TestCaseData ("\"アド\"レス\" <aaaa@example.com>"),
				//new TestCaseData ("ア,ド;レ.ス@あ-ど-れ_す")
			};
			return addressList;
		}

		[TestCaseSource ("LegacyAddressNotCompliantWithRFC")]
		public void TestLegacyEmailAddress (string address)
		{
			string text = address;

			AssertParse (text);
		}
		#endregion
	}
}
