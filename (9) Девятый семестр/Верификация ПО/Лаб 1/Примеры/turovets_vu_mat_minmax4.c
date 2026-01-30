// turovets_vu_mat_minmax4.c

/*@
  predicate in_matrix(int **mat, integer n, integer m, integer val) =
    \exists integer r, c;
      0 <= r < n && 0 <= c < m && mat[r][c] == val;

  predicate before_rc(integer i, integer j, integer r, integer c) =
    (r < i) || (r == i && c < j);

  predicate prefix_lower(int **mat, integer n, integer m,
                         integer i, integer j, integer val) =
    \forall integer r, c;
      0 <= r < n && 0 <= c < m && before_rc(i, j, r, c) ==> mat[r][c] >= val;

  predicate prefix_upper(int **mat, integer n, integer m,
                         integer i, integer j, integer val) =
    \forall integer r, c;
      0 <= r < n && 0 <= c < m && before_rc(i, j, r, c) ==> mat[r][c] <= val;

  predicate matrix_lower(int **mat, integer n, integer m, integer val) =
    prefix_lower(mat, n, m, n, 0, val);

  predicate matrix_upper(int **mat, integer n, integer m, integer val) =
    prefix_upper(mat, n, m, n, 0, val);
*/

/*@
  requires n > 0;
  requires m > 0;
  requires \valid(mat + (0 .. n - 1));
  requires \forall integer i; 0 <= i < n ==> \valid(mat[i] + (0 .. m - 1));
  requires \valid(min);
  requires \valid(max);
  requires \separated(min, max);

  assigns *min, *max;

  ensures in_matrix(mat, n, m, *min);
  ensures in_matrix(mat, n, m, *max);
  ensures matrix_lower(mat, n, m, *min);
  ensures matrix_upper(mat, n, m, *max);
  ensures *min <= *max;
*/
void mat_minmax_4(int **mat, int n, int m, int *min, int *max) {
    int mmin = mat[0][0];
    int mmax = mat[0][0];

    /*@ assert in_matrix(mat, n, m, mmin); */
    /*@ assert in_matrix(mat, n, m, mmax); */
    /*@ assert mmin <= mmax; */

    /*@
      loop invariant 0 <= i <= n;
      loop invariant prefix_lower(mat, n, m, i, 0, mmin);
      loop invariant prefix_upper(mat, n, m, i, 0, mmax);
      loop invariant in_matrix(mat, n, m, mmin);
      loop invariant in_matrix(mat, n, m, mmax);
      loop invariant mmin <= mmax;

      loop assigns mmin, mmax, i;
      loop variant n - i;
    */
    for (int i = 0; i < n; ++i) {
        /*@
          loop invariant 0 <= j <= m;
          loop invariant 0 <= i < n;
          loop invariant prefix_lower(mat, n, m, i, j, mmin);
          loop invariant prefix_upper(mat, n, m, i, j, mmax);
          loop invariant in_matrix(mat, n, m, mmin);
          loop invariant in_matrix(mat, n, m, mmax);
          loop invariant mmin <= mmax;

          loop assigns mmin, mmax, j;
          loop variant m - j;
        */
        for (int j = 0; j < m; ++j) {
            int v = mat[i][j];
            /*@ assert in_matrix(mat, n, m, v); */
            if (v < mmin) {
                mmin = v;
            } else if (v > mmax) {
                mmax = v;
            }
            /*@ assert mmin <= mmax; */
        }
        /*@ assert prefix_lower(mat, n, m, i, m, mmin); */
        /*@ assert prefix_upper(mat, n, m, i, m, mmax); */
        /*@ assert prefix_lower(mat, n, m, i + 1, 0, mmin); */
        /*@ assert prefix_upper(mat, n, m, i + 1, 0, mmax); */
    }
    *min = mmin;
    *max = mmax;
}
