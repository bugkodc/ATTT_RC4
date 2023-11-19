using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.TextCore.Text;
using System.Text;
using System;
using Unity.VisualScripting;
public class Managerinput : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI _input_p;
    [SerializeField] public TextMeshProUGUI _input_q;
    [SerializeField] public TextMeshProUGUI _input_s;
    [SerializeField] public TextMeshProUGUI _input_n;
    [SerializeField] public TextMeshProUGUI _input_e;
    [SerializeField] public TextMeshProUGUI _input_d;
    [SerializeField] public TextMeshProUGUI _input_banro;
    [SerializeField] public TextMeshProUGUI _input_banma;
    [SerializeField] public TextMeshProUGUI _output_banma;
    [SerializeField] public TextMeshProUGUI _output_banro;
    public int _danhDau = 0;
    private bool nguyenToCungNhau(int ai, int bi)// "Hàm kiểm tra hai số nguyên tố cùng nhau"
    {
        bool ktx_;
        // giải thuật Euclid;
        int temp;
        while (bi != 0)
        {
            temp = ai % bi;
            ai = bi;
            bi = temp;
        }
        if (ai == 1) { ktx_ = true; }
        else ktx_ = false;
        return ktx_;
    }
    int _input_P, _input_Q, _input_N, _input_E, _intput_D, _intput_S;
    string _intput_banma , _intput_banro ;
    public int RSA_d_dau = 0;
    int _RSA_N, _RSA_SoPhi_N, _RSA_E, _RSA_D;
    string _str_N, _str_SoPhi_N, _str_E, _str_D, _str_BanMa, _str_BanGiaiMa;
    #region Input
    private void Inputsystem()
    {
        _input_P = Int32.Parse(_input_p.text);
        _input_Q = Int32.Parse(_input_q.text);
        _input_N = Int32.Parse(_input_n.text);
        _input_E = Int32.Parse(_input_e.text);
        _intput_D = Int32.Parse(_input_d.text);
        _intput_S = Int32.Parse(_input_s.text);
        _intput_banma = _input_banma.ToString();
        _intput_banro = _input_banro.ToString();
    }
    #endregion
    #region random_number
    private int RSA_ChonSoNgauNhien()
    {
        System.Random rd = new System.Random();
        return rd.Next(11, 101);// tốc độ chậm nên chọn số bé
    }
    #endregion
    #region so_Nguyento
    private bool RSA_kiemTraNguyenTo(int xi)
    {
        bool kiemtra = true;
        if (xi == 2 || xi == 3)
        {
            // kiemtra = true;
            return kiemtra;
        }
        else
        {
            if (xi == 1 || xi % 2 == 0 || xi % 3 == 0)
            {
                kiemtra = false;
            }
            else
            {
                for (int i = 5; i <= Math.Sqrt(xi); i = i + 6)
                    if (xi % i == 0 || xi % (i + 2) == 0)
                    {
                        kiemtra = false;
                        break;
                    }
            }
        }
        return kiemtra;
    }
    // "Hàm kiểm tra hai số nguyên tố cùng nhau"
    private bool RSA_nguyenToCungNhau(int ai, int bi)
    {
        bool ktx_;
        // giải thuật Euclid;
        int temp;
        while (bi != 0)
        {
            temp = ai % bi;
            ai = bi;
            bi = temp;
        }
        if (ai == 1) { ktx_ = true; }
        else ktx_ = false;
        return ktx_;
    }
    #endregion
    #region so_mod
    // "Hàm lấy mod"
    public int RSA_mod(int mx, int ex, int nx)
    {

        //Sử dụng thuật toán "bình phương nhân"
        //Chuyển e sang hệ nhị phân
        int[] a = new int[100];
        int k = 0;
        do
        {
            a[k] = ex % 2;
            k++;
            ex = ex / 2;
        }
        while (ex != 0);
        //Quá trình lấy dư
        int kq = 1;
        for (int i = k - 1; i >= 0; i--)
        {
            kq = (kq * kq) % nx;
            if (a[i] == 1)
                kq = (kq * mx) % nx;
        }
        return kq;
    }
    #endregion
    #region tao_khoa
    private void RSA_taoKhoa()
    {
        //Tinh n=p*q
        _RSA_N = _input_P * _input_Q;
        _str_N = _RSA_N.ToString();
        //Tính Phi(n)=(p-1)*(q-1)
        _RSA_SoPhi_N = (_input_P - 1) * (_input_Q - 1);
        _str_SoPhi_N = _RSA_SoPhi_N.ToString();
        //Tính e là một số ngẫu nhiên có giá trị 1< e <phi(n) và là số nguyên tố cùng nhau với Phi(n)
        do
        {
            System.Random RSA_rd = new System.Random();
            _RSA_E = RSA_rd.Next(2, _RSA_SoPhi_N);
        }
        while (!nguyenToCungNhau(_RSA_E, _RSA_SoPhi_N));
        _str_E = _RSA_E.ToString();

        //Tính d là nghịch đảo modular của e
        _RSA_D = 0;
        int i = 2;
        while (((1 + i * _RSA_SoPhi_N) % _RSA_E) != 0 || _RSA_D <= 0)
        {
            i++;
            _RSA_D = (1 + i * _RSA_SoPhi_N) / _RSA_E;
        }
        _str_D = _RSA_D.ToString();
    }
    #endregion
    public void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Quit()
    {
        Debug.Log("can");
        Application.Quit();
    }
}
