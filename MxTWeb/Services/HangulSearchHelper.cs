using System.Text.RegularExpressions;

namespace MxTWeb.Services
{
    public static class HangulSearchHelper
    {
        // 초성 배열 (19개)
        private static readonly char[] CHOSUNG = {
        'ㄱ', 'ㄲ', 'ㄴ', 'ㄷ', 'ㄸ', 'ㄹ', 'ㅁ', 'ㅂ', 'ㅃ', 'ㅅ',
        'ㅆ', 'ㅇ', 'ㅈ', 'ㅉ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ'
    };

        // 중성 배열 (21개)
        private static readonly char[] JUNGSUNG = {
        'ㅏ', 'ㅐ', 'ㅑ', 'ㅒ', 'ㅓ', 'ㅔ', 'ㅕ', 'ㅖ', 'ㅗ', 'ㅘ',
        'ㅙ', 'ㅚ', 'ㅛ', 'ㅜ', 'ㅝ', 'ㅞ', 'ㅟ', 'ㅠ', 'ㅡ', 'ㅢ', 'ㅣ'
    };

        // 종성 배열 (28개, 받침 없음 포함)
        private static readonly char[] JONGSUNG = {
        ' ', 'ㄱ', 'ㄲ', 'ㄳ', 'ㄴ', 'ㄵ', 'ㄶ', 'ㄷ', 'ㄹ', 'ㄺ',
        'ㄻ', 'ㄼ', 'ㄽ', 'ㄾ', 'ㄿ', 'ㅀ', 'ㅁ', 'ㅂ', 'ㅄ', 'ㅅ',
        'ㅆ', 'ㅇ', 'ㅈ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ'
    };

        /// <summary>
        /// 한글 문자를 초성, 중성, 종성으로 분해
        /// </summary>
        public static (char chosung, char jungsung, char jongsung) DecomposeHangul(char hangul)
        {
            if (hangul < 0xAC00 || hangul > 0xD7A3)
                return (' ', ' ', ' '); // 한글이 아님

            int code = hangul - 0xAC00;
            int chosungIndex = code / (21 * 28);
            int jungsungIndex = (code % (21 * 28)) / 28;
            int jongsungIndex = code % 28;

            return (CHOSUNG[chosungIndex], JUNGSUNG[jungsungIndex], JONGSUNG[jongsungIndex]);
        }

        /// <summary>
        /// 초성, 중성, 종성으로 한글 문자 조합
        /// </summary>
        public static char ComposeHangul(char chosung, char jungsung, char jongsung = ' ')
        {
            int chosungIndex = Array.IndexOf(CHOSUNG, chosung);
            int jungsungIndex = Array.IndexOf(JUNGSUNG, jungsung);
            int jongsungIndex = Array.IndexOf(JONGSUNG, jongsung);

            if (chosungIndex == -1 || jungsungIndex == -1 || jongsungIndex == -1)
                return ' ';

            int code = 0xAC00 + (chosungIndex * 21 * 28) + (jungsungIndex * 28) + jongsungIndex;
            return (char)code;
        }

        /// <summary>
        /// 초성 검색용 정규식 생성
        /// </summary>
        public static string CreateChosungRegex(string input)
        {
            if (string.IsNullOrEmpty(input))
                return ".*";

            var regexPattern = "";

            foreach (char c in input)
            {
                if (c >= 0xAC00 && c <= 0xD7A3) // 완성형 한글
                {
                    var (chosung, jungsung, jongsung) = DecomposeHangul(c);

                    if (jongsung == ' ') // 받침 없음
                    {
                        // 현재 글자와 같거나, 이 글자에 받침이 추가된 형태 모두 매칭
                        var baseChar = ComposeHangul(chosung, jungsung);
                        var endChar = ComposeHangul(chosung, jungsung, 'ㅎ'); // 마지막 받침
                        regexPattern += $"[{baseChar}-{endChar}]";
                    }
                    else
                    {
                        // 받침이 있는 경우 정확히 매칭
                        regexPattern += c;
                    }
                }
                else if (IsChosung(c)) // 순수 초성 (ㄱ, ㄴ, ㄷ 등)
                {
                    // 해당 초성으로 시작하는 모든 한글 매칭
                    var startChar = ComposeHangul(c, 'ㅏ'); // 가, 나, 다...
                    var endChar = ComposeHangul(c, 'ㅣ', 'ㅎ'); // 깋, 닣, 딯...
                    regexPattern += $"[{startChar}-{endChar}]";
                }
                else
                {
                    // 영문, 숫자 등은 그대로
                    regexPattern += Regex.Escape(c.ToString());
                }
            }

            return regexPattern;
        }

        /// <summary>
        /// 초성인지 확인
        /// </summary>
        private static bool IsChosung(char c)
        {
            return CHOSUNG.Contains(c);
        }

        /// <summary>
        /// 초성 검색 수행
        /// </summary>
        public static List<string> SearchByChosung(List<string> dataList, string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return dataList;

            var regexPattern = CreateChosungRegex(searchTerm);
            var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);

            return dataList.Where(item => regex.IsMatch(item)).ToList();
        }

        /// <summary>
        /// 미완성 한글 처리 (카카오톡 스타일)
        /// </summary>
        public static string ProcessIncompleteHangul(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var lastChar = input[^1];

            // 마지막 글자가 한글이고 받침이 있는 경우
            if (lastChar >= 0xAC00 && lastChar <= 0xD7A3)
            {
                var (chosung, jungsung, jongsung) = DecomposeHangul(lastChar);

                if (jongsung != ' ') // 받침이 있으면
                {
                    // 받침을 다음 글자의 초성으로 처리
                    var baseWord = input[..^1] + ComposeHangul(chosung, jungsung); // 받침 제거
                    var nextChosung = jongsung; // 받침을 초성으로

                    return baseWord + nextChosung;
                }
            }

            return input;
        }
    }

    // 사용 예제
    public class SearchExample
    {
        private List<string> sampleData = new List<string>
        {
            "스키장", "스케이트", "스코어", "스카이", "스킨케어",
            "카카오톡", "카페", "카드", "카메라", "카누",
            "김치", "김밥", "김치찌개", "김치볶음밥"
        };

        public List<string> Search(string query)
        {
            // 미완성 한글 처리 (슼 → 스ㅋ)
            var processedQuery = HangulSearchHelper.ProcessIncompleteHangul(query);

            // 초성 검색 수행
            return HangulSearchHelper.SearchByChosung(sampleData, processedQuery);
        }
    }

}
