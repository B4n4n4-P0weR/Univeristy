// turovets_vu_oddeven_lr.c
// 13

#include <limits.h>

/*@ predicate swap_in_array{L1, L2}(int* a, integer b, integer e, integer i, integer j) =
  @     b <= i < e && b <= j < e &&
  @     \at(a[i], L1) == \at(a[j], L2) &&
  @     \at(a[j], L1) == \at(a[i], L2) &&
  @     \forall integer k; b <= k < e && k != i && k != j ==>
  @         \at(a[k], L1) == \at(a[k], L2);
  @
  @ inductive permutation{L1, L2}(int* a, integer b, integer e) {
  @     case reflexive{L1}:
  @         \forall int* a, integer b, e;
  @             permutation{L1, L1}(a, b, e);
  @     case swap{L1, L2}:
  @         \forall int* a, integer b, e, i, j;
  @             swap_in_array{L1, L2}(a, b, e, i, j) ==>
  @                 permutation{L1, L2}(a, b, e);
  @     case transitive{L1, L2, L3}:
  @         \forall int* a, integer b, e;
  @             permutation{L1, L2}(a, b, e) && permutation{L2, L3}(a, b, e) ==>
  @                 permutation{L1, L3}(a, b, e);
  @ }
  @
  @ predicate unchanged{L1, L2}(int *a, integer b, integer e) =
  @     \forall integer i; b <= i < e ==>
  @         \at(a[i], L1) == \at(a[i], L2);
  @
  @ predicate sorted(int* a, integer b, integer e) =
  @     \forall integer i, j;
  @         b <= i <= j < e ==> a[i] <= a[j];
  @
  @ predicate element_level_sorted(int* a, integer b, integer e) =
  @     \forall integer i;
  @         b <= i < e - 1 ==> a[i] <= a[i + 1];
  @
  @ predicate even_pairs_sorted(int* a, integer b, integer e) =
  @     \forall integer i;
  @         b <= i < e - 1 && ((i - b) % 2 == 0) ==> a[i] <= a[i + 1];
  @
  @ predicate odd_pairs_sorted(int* a, integer b, integer e) =
  @     \forall integer i;
  @         b <= i < e - 1 && ((i - b) % 2 == 1) ==> a[i] <= a[i + 1];
  @*/

/*@ lemma even_and_odd_sorted_help:
  @     \forall int* a, integer b, e;
  @         (\forall integer i; b <= i < e - 1 && ((i - b) % 2 == 0) ==> a[i] <= a[i + 1]) &&
  @         (\forall integer i; b <= i < e - 1 && ((i - b) % 2 == 1) ==> a[i] <= a[i + 1]) ==>
  @             \forall integer j; b <= j < e - 1 ==> a[j] <= a[j + 1];
  @
  @ lemma even_and_odd_sorted:
  @     \forall int* a, integer b, e;
  @         even_pairs_sorted(a, b, e) && odd_pairs_sorted(a, b, e) ==>
  @             element_level_sorted(a, b, e);
  @
  @ lemma odd_pairs_sorted_extend_even_end:
  @     \forall int* a, integer b, e;
  @         b <= e && odd_pairs_sorted(a, b, e) && ((e - b) % 2 == 1) ==>
  @             odd_pairs_sorted(a, b, e + 1);
  @
  @ lemma even_pairs_sorted_extend_odd_end:
  @     \forall int* a, integer b, e;
  @         b <= e && even_pairs_sorted(a, b, e) && ((e - b) % 2 == 0) ==>
  @             even_pairs_sorted(a, b, e + 1);
  @
  @ lemma even_pairs_sorted_extend_even_end:
  @     \forall int* a, integer b, e;
  @         b <= e && even_pairs_sorted(a, b, e) && ((e - b) % 2 == 0) && a[e] <= a[e + 1] ==>
  @             even_pairs_sorted(a, b, e + 2);
  @
  @*/

/*@ lemma int_minus_one_lt:
  @     \forall integer x; x - 1 < x;
  @*/

#define element_level_sorted_implies_sorted(_arr, _len)                                 \
/@ assert 1 <= _len <= INT_MAX; @/                                                      \
/@ assert element_level_sorted(_arr, 0, _len); @/                                       \
/@ loop invariant 1 <= _i <= _len;                                                     \
   loop invariant sorted(_arr, 0, _i);                                                  \
   loop assigns _i;                                                                     \
   loop variant _len - _i; @/                                                           \
   for(int _i = 1; _i < _len ; ++_i){                                                   \
       /@ assert _arr[_i - 1] <= _arr[_i]; @/                                           \
   }                                                                                    \
/@ assert sorted(_arr, 0, _len); @/

/*@ logic integer less_than_count{L}(int* a, integer b, integer e, int val) =
  @     (b >= e) ? 0 :
  @         ((\at(a[b], L) < val ? 1 : 0) + less_than_count{L}(a, b + 1, e, val));
  @
  @ logic integer inverses{L}(int* a, integer b, integer e) =
  @     (b >= e) ? 0 :
  @         (less_than_count{L}(a, b + 1, e, a[b]) + inverses{L}(a, b + 1, e));
  @
  @ lemma swap_dec_inv_by_1{L1, L2}:
  @     \forall int* a, integer b, e, i, j;
  @         (b <= i < j < e) && (\at(a[i], L1) > \at(a[j], L1)) && swap_in_array{L1, L2}(a, b, e, i, j) ==>
  @             inverses{L2}(a, b, e) == inverses{L1}(a, b, e) - 1;
  @
  @*/

/*@
  @ lemma lt_count_zero_test:
  @     \forall int* a, integer b, e, int val;
  @         (b >= e) ==> less_than_count(a, b, e, val) == 0;
  @
  @ lemma inv_zero_test:
  @     \forall int* a, integer b, e;
  @         (b >= e) ==> inverses(a, b, e) == 0;
  @
  @ lemma inv_zero_test_2:
  @     \forall int* a, integer b, e;
  @         (b < e) && (b + 1 == e) ==> inverses(a, b, e) == 0;
  @
  @ lemma lt_count_inc{L}:
  @     \forall int* a, integer b, e, int val;
  @         (b < e) && (\at(a[b], L)  < val) ==> less_than_count{L}(a, b, e, val) == less_than_count{L}(a, b + 1, e, val) + 1;
  @
  @ lemma lt_count_eq{L}:
  @     \forall int* a, integer b, e, int val;
  @         (b < e) && (\at(a[b], L) >= val) ==> less_than_count{L}(a, b, e, val) == less_than_count{L}(a, b + 1, e, val);
  @
  @ lemma inv_inc{L}:
  @     \forall int* a, integer b, e;
  @         (b < e) ==> inverses{L}(a, b, e) == inverses{L}(a, b + 1, e) + less_than_count{L}(a, b + 1, e, a[b]);
  @*/

#define inverses_is_gt_zero(_arr, _len)                                                                                                         \
/@ assert 1 <= _len <= INT_MAX; @/                                                                                                              \
/@ assert \valid(_arr + (0.._len-1)); @/                                                                                                        \
/@ loop invariant 0 <= _j <= _len - 1 ;                                                                                                         \
   loop invariant inverses(_arr, _j, _len) >= 0;                                                                                                \
   loop invariant \forall int _val; less_than_count(_arr, _j, _len, _val) >= 0;                                                                 \
   loop invariant (_arr[_j - 1] >= _arr[_j]) ==> (less_than_count(_arr, _j, _len, _arr[_j]) == less_than_count(_arr, _j + 1, _len, _arr[_j]));  \
   loop invariant (_arr[_j - 1] < _arr[_j]) ==> (less_than_count(_arr, _j, _len, _arr[_j]) >= less_than_count(_arr, _j + 1, _len, _arr[_j]));   \
   loop assigns _j;                                                                                                                             \
   loop variant _j; @/                                                                                                                          \
   for(int _j = _len - 1; _j > 0; --_j) {                                                                                                       \
       /@ assert (_j < _len - 1) ==> inverses(_arr, _j, _len) >= inverses(_arr, _j + 1, _len); @/                                               \
       /@ assert inverses(_arr, _j, _len) >= 0; @/                                                                                              \
   }                                                                                                                                            \
/@ assert inverses(_arr, 0, _len) >= 0; @/

/*@ requires 1 <= n <= INT_MAX;
  @ requires \valid(arr + (0..n-1));
  @
  @ assigns arr[0..n-1];
  @
  @ ensures permutation{Pre, Post}(arr, 0, n);
  @ ensures sorted(arr, 0, n);
  @*/
void oddeven_lr(int *arr, int n) {
    int i, tmp, cnt = 1;
    //@ ghost inverses_is_gt_zero(arr, n);
    //@ assert inverses(arr, 0, n) >= 0;
    /*@ loop invariant permutation{LoopEntry, Here}(arr, 0, n);
      @ loop invariant 0 <= cnt <= n;
      @ loop invariant (cnt == 0) ==> element_level_sorted(arr, 0, n);
      @ loop invariant inverses(arr, 0, n) >= 0;
      @ loop variant inverses{Here}(arr, 0, n) + ((cnt > 0) ? 0 : -1);
      @*/
    while (cnt > 0) {
        cnt = 0;
        /*@ loop invariant permutation{LoopEntry, Here}(arr, 0, n);
          @ loop invariant 1 <= i <= n;
          @ loop invariant (i - 1) % 2 == 0;
          @ loop invariant odd_pairs_sorted(arr, 0, i);
          @ loop invariant 0 <= cnt;
          @ loop invariant 2 * cnt <= i;
          @ loop invariant (cnt == 0) ==> (inverses{Here}(arr, 0, n) == inverses{LoopEntry}(arr, 0, n));
          @ loop invariant (cnt > 0) ==> (inverses{Here}(arr, 0, n) < inverses{LoopEntry}(arr, 0, n));
          @ loop variant n - i;
          @*/
        for (i = 1; i <= n - 2; i = i + 2) {
            if (arr[i] > arr[i + 1]) {
                tmp = arr[i];
                arr[i] = arr[i + 1];
                arr[i + 1] = tmp;
                //@ assert 0 <= i < i + 1 < n;
                //@ assert \at(arr[i], LoopCurrent) > \at(arr[i + 1], LoopCurrent);
                //@ assert swap_in_array{LoopCurrent, Here}(arr, 0, n, i, i + 1);
                //@ assert inverses{Here}(arr, 0, n) == inverses{LoopCurrent}(arr, 0, n) - 1;
                ++cnt;
            }
            //@ assert arr[i] <= arr[i + 1];
            //@ assert 2 * cnt <= i + 2;
            //@ assert i + 2 <= n;
            //@ assert n - (i + 2) < n - i;
        }
        //@ assert odd_pairs_sorted(arr, 0, i);
        //@ assert n - 1 <= i <= n;
        //@ assert (i % 2 == 1);
        //@ assert (i < n) ==> odd_pairs_sorted(arr, 0, i + 1);
        //@ assert (i < n) ==> (i + 1 == n);
        //@ assert odd_pairs_sorted(arr, 0, n);
        //@ ghost int prev_cnt = cnt;
        /*@ loop invariant permutation{LoopEntry, Here}(arr, 0, n);
          @ loop invariant 0 <= i <= n;
          @ loop invariant (i % 2 == 0);
          @ loop invariant even_pairs_sorted(arr, 0, i);
          @ loop invariant 0 <= cnt - prev_cnt;
          @ loop invariant 2 * (cnt - prev_cnt) <= i;
          @ loop invariant (cnt == prev_cnt) ==> odd_pairs_sorted(arr, 0, n);
          @ loop invariant (cnt == prev_cnt) ==> (inverses{Here}(arr, 0, n) == inverses{LoopEntry}(arr, 0, n));
          @ loop invariant (cnt > prev_cnt) ==> (inverses{Here}(arr, 0, n) < inverses{LoopEntry}(arr, 0, n));
          @ loop variant n - i;
          @*/
        for (i = 0; i <= n - 2; i = i + 2) {
            if (arr[i] > arr[i + 1]) {
                tmp = arr[i];
                arr[i] = arr[i + 1];
                arr[i + 1] = tmp;
                //@ assert 0 <= i < i + 1 < n;
                //@ assert \at(arr[i], LoopCurrent) > \at(arr[i + 1], LoopCurrent);
                //@ assert swap_in_array{LoopCurrent, Here}(arr, 0, n, i, i + 1);
                //@ assert inverses{Here}(arr, 0, n) == inverses{LoopCurrent}(arr, 0, n) - 1;
                ++cnt;
            }
            //@ assert arr[i] <= arr[i + 1];
            //@ assert 0 <= i;
            //@ assert (i - 0) % 2 == 0;
            //@ assert even_pairs_sorted(arr, 0, i) && arr[i] <= arr[i + 1] && ((i - 0) % 2 == 0) ==> even_pairs_sorted(arr, 0, i + 2);
            //@ assert even_pairs_sorted(arr, 0, i + 2);
            //@ assert 2 * (cnt - prev_cnt) <= i + 2;
            //@ assert i + 2 <= n;
            //@ assert n - (i + 2) < n - i;
        }
        //@ assert even_pairs_sorted(arr, 0, i);
        //@ assert n - 1 <= i <= n;
        //@ assert (i % 2 == 0);
        //@ assert (i < n) ==> even_pairs_sorted(arr, 0, i + 1);
        //@ assert (i < n) ==> (i + 1 == n);
        //@ assert even_pairs_sorted(arr, 0, n);
        //@ ghost inverses_is_gt_zero(arr, n);
        //@ assert (cnt == prev_cnt) ==> odd_pairs_sorted(arr, 0, n);
        //@ assert (cnt == 0) ==> odd_pairs_sorted(arr, 0, n);
        //@ assert (cnt == 0) ==> element_level_sorted(arr, 0, n);
        //@ assert (cnt == 0) ==> inverses{Here}(arr, 0, n) == inverses{LoopCurrent}(arr, 0, n);
        //@ assert (cnt > 0) ==> inverses{Here}(arr, 0, n) < inverses{LoopCurrent}(arr, 0, n);
        //@ assert (cnt == 0) ==> inverses{Here}(arr, 0, n) - 1 < inverses{LoopCurrent}(arr, 0, n);
        //@ assert inverses{Here}(arr, 0, n) + ((cnt > 0) ? 0 : -1) < inverses{LoopCurrent}(arr, 0, n);
    }
    //@ assert cnt == 0;
    //@ ghost element_level_sorted_implies_sorted(arr, n);
    //@ assert sorted(arr, 0, n);
}
