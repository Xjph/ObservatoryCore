$cert = Get-ChildItem Cert:\LocalMachine\My -CodeSigningCert
Set-AuthenticodeSignature -FilePath ./ObservatoryCore/bin/Release/net6.0-windows/ObservatoryCore.exe -Certificate $cert
Set-AuthenticodeSignature -FilePath ./ObservatoryCore/bin/Release/net6.0-windows/plugins/ObservatoryExplorer.dll -Certificate $cert
Set-AuthenticodeSignature -FilePath ./ObservatoryCore/bin/Release/net6.0-windows/plugins/ObservatoryBotanist.dll -Certificate $cert
Set-AuthenticodeSignature -FilePath ./ObservatoryCore/bin/Release/net6.0-windows/plugins/ObservatoryHerald.dll -Certificate $cert
# SIG # Begin signature block
# MIIF0AYJKoZIhvcNAQcCoIIFwTCCBb0CAQExCzAJBgUrDgMCGgUAMGkGCisGAQQB
# gjcCAQSgWzBZMDQGCisGAQQBgjcCAR4wJgIDAQAABBAfzDtgWUsITrck0sYpfvNR
# AgEAAgEAAgEAAgEAAgEAMCEwCQYFKw4DAhoFAAQUjqIsfdT62QvQPP+a5YOXVRCf
# 3o6gggNKMIIDRjCCAi6gAwIBAgIQQbRFPs6oPodFBj0fsFanmzANBgkqhkiG9w0B
# AQsFADA7MRgwFgYDVQQDDA9Kb25hdGhhbiBNaWxsZXIxHzAdBgkqhkiG9w0BCQEW
# EGptaWxsZXJAeGpwaC5uZXQwHhcNMjMwMzI4MTgxNTM3WhcNMjQwMzI4MTgzNTM3
# WjA7MRgwFgYDVQQDDA9Kb25hdGhhbiBNaWxsZXIxHzAdBgkqhkiG9w0BCQEWEGpt
# aWxsZXJAeGpwaC5uZXQwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQCv
# eeKY2sY4SAMLgjE+sm1lj8Tje5QArsuSqLFC0gpWzHFq2HZYHLGR5l9Kz1Jm4iNm
# bdkQiEtt5o6e48L2GLqftM0XklmkNVzyuj6SqL99K1JCuO/kLRVorqRV/88NpOOe
# Bpn1W5FTA7m1PVCYXbz3a6l93hNY6mI4yb9MV8nKFDnmmAtiwIsKgXuNf81sU8bg
# 4A7mB9A7Jgvx1/Gs7rFu0m1qWIGpfhsh8EQtpJaiVvzCBqdpIvDEnMwlVd6S0nkj
# jCCB7s12oiXKYjBS1Vm1YfwoaPkHe9E+z7zgHnhZ5hrTt8g/TZM+cS2o+5JQYTr9
# RZUjQ3EmsUfZMAuSekERAgMBAAGjRjBEMA4GA1UdDwEB/wQEAwIFoDATBgNVHSUE
# DDAKBggrBgEFBQcDAzAdBgNVHQ4EFgQUCeFpbq5cQi1Z5DQydkmiF8MIfyMwDQYJ
# KoZIhvcNAQELBQADggEBAHF/lRtemEggwTFiwTI01Z3erV6YGNT2miD4NrUrnQEe
# kI+Ezh/MLj2vRmqeVz7XX1ePZX0sd7sViRMnPm+LTl8UltZqhTWV/h7qmi/2Vf74
# QHLE/Ht3olWBdGOVzeeP5XLMBqqg7HWPHGpTA8lx0ApI4YhYu7w/SgwzYUj2NF2O
# GRmV78kcHeYf+h5lZzAKjc+dgH+ucsqpKgDxCk8lBhUkd102YGMUZophz0L8RTD4
# k/CAliVZo3m8ENsR6pMnjsgifeZ8Q9ydpBXawIdcqW9xtZanvYN9+GAHMYeFWWBf
# 0fBcoPAy4X5bcvQmK/0d7znpgDmgm4jYywF5ptHXoAIxggHwMIIB7AIBATBPMDsx
# GDAWBgNVBAMMD0pvbmF0aGFuIE1pbGxlcjEfMB0GCSqGSIb3DQEJARYQam1pbGxl
# ckB4anBoLm5ldAIQQbRFPs6oPodFBj0fsFanmzAJBgUrDgMCGgUAoHgwGAYKKwYB
# BAGCNwIBDDEKMAigAoAAoQKAADAZBgkqhkiG9w0BCQMxDAYKKwYBBAGCNwIBBDAc
# BgorBgEEAYI3AgELMQ4wDAYKKwYBBAGCNwIBFTAjBgkqhkiG9w0BCQQxFgQUfXa2
# HgFmPrFbD6PqO1Z7s6yzLkowDQYJKoZIhvcNAQEBBQAEggEASEcYW2MnOLX+dMin
# lEVxL8fTOrf/XuE05QwqQSHOBqqO4GMuR+IeBjE2R4EjxQsZMQVok1dK302ByHA9
# OVv37xG4exqP/vkP3NX/z2s1Cl2PE1gzxVNgdGlbkzIQF9EiMXr9P/QGifCg2TLV
# 2mk4Vt+mA1/tU066tNXahbL9N9b+yLcB3VNfru/SnvO/ZPzKCmjNZW54mnNKnRCE
# PJDVKEKla/ufh8iMR+SiIaaXrwypvdz8CYK9OSs9qr0Cjp9jY1TXLxgNZiTenUoY
# n+sVQzv+N1PAy2nvSXlnesbxlO3T2XPp6fYkpj1uYCoun3Ztpr2MoKRKBgzybo7Z
# GYn3QA==
# SIG # End signature block